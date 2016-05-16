using System;
using System.Collections.Generic;
using System.Linq;
namespace Ad_graph
{

   struct Transition_prob
   {
      public double for_click,for_price,for_end;
   };



   struct State
   {
      private double  pclick, relevance, bid;
    
      public  double reward;
    
      public int best_action;  //this will be used for policy pi->a
    
      public double state_value; //this will be used ofr storing the value of a state

      public Transition_prob t_p;


      public State(double pclick_, double relevance_,double bid_):this()
         {
            this.pclick=pclick_; 
            this.relevance=relevance_;
            this.bid=bid_; 

            //calulating the reward and transition probability
            set_reward();
            set_transition_prob();
         } 

      public double [] get_feature()
        {   
            double[] feature_vector ={pclick,relevance,bid};
            return feature_vector;
            
        }   
    
      public void set_reward()
      {   //some random way
           reward=pclick*.8+relevance*.1+bid*.1;
       }

      public void set_transition_prob()
      {
         t_p.for_click=.8*pclick;
         t_p.for_price=1- pclick;
         t_p.for_end=.2*pclick;
      } 

      public void set_best_action(int best)
      {
        best_action=best;
      }

      public void set_state_value(double value_)
      {
        state_value=value_;
      }

      public double get_state_value()
      {
        return state_value;
      }

   };//end of struct feature 
   
   

   
   class AdjacencyList
    {
        public LinkedList<Tuple<int, State>>[] adjacencyList;
        
        // Constructor - creates an empty Adjacency List
        public AdjacencyList(int vertices)
        {
            adjacencyList = new LinkedList<Tuple<int, State>>[vertices];

 
            for (int i = 0; i < adjacencyList.Length; ++i)
            {
                adjacencyList[i] = new LinkedList<Tuple<int, State>>();
            }
        }
 
        // Appends a new Edge to the linked list
        public void addEdgeAtEnd(int startVertex, int endVertex, State s)
        {
            adjacencyList[startVertex].AddLast(new Tuple<int, State>(endVertex, s));
        }
 
        // Adds a new Edge to the linked list from the front
        public void addEdgeAtBegin(int startVertex, int endVertex, State s)
        {
            adjacencyList[startVertex].AddFirst(new Tuple<int, State>(endVertex, s));
        }
 
 
        // Returns a copy of the Linked List of outward edges from a vertex
         public LinkedList<Tuple<int, State>> get_linked_nodes(int index)
         {
            
            LinkedList<Tuple<int, State>> edgeList
                               = new LinkedList<Tuple<int, State>>(adjacencyList[index]);
            return edgeList;
         }
 

        public State get_node(int index)
        {
        //Console.Write(ad_graph.adjacencyList[0].GetType() ==  typeof(ad_graph.adjacencyList[index]));
          return adjacencyList[index].ElementAt(index).Item2;
        }      

        // Prints the Adjacency List
        public void printAdjacencyList()
        {
            int i = 0;
 
            foreach (LinkedList<Tuple<int, State>> list in adjacencyList)
            {
                Console.Write("adjacencyList[" + i + "] -> ");
 
                foreach (Tuple<int, State> edge in list)
                {
                    Console.Write(edge.Item1 + "(" + edge.Item2.reward + " "+edge.Item2.t_p.for_price +")  ");
                }
 
                ++i;
                Console.WriteLine();
            }
        }
 
        
    }
 
   class MDP 
   {

      public AdjacencyList ad_graph;
      int no_states =100;
      double gamma=0.6;
      //constructor      
      public MDP()
      {
         ad_graph=initialize_graph();   
      }

      public AdjacencyList initialize_graph()
      {
         
         
         double relevance,bid,pclick;
         
         Random rnd = new Random();
         AdjacencyList ad_graph = new AdjacencyList(no_states);

         //randomly initialize the graph
         for(int i=0;i<100;i++)
         {   
            
            for (int j=0;j<100;j++)
                {   
                    pclick = (double)rnd.Next(1, 101)/100; 
                    relevance= (double)rnd.Next(1, 101)/100;
                    bid=(double)rnd.Next(1, 101)/100;
                    ad_graph.addEdgeAtEnd(i, j,new State(pclick,relevance,bid));
                }    
         }

         return ad_graph;
      }

      public void  show_adjacent_nodes(int index)
      {
          LinkedList<Tuple<int, State>> edgeList=ad_graph.get_linked_nodes(index);
      
          foreach (Tuple<int, State> edge in edgeList)
                {
                    Console.Write(edge.Item1 + "(" + edge.Item2.reward + " "+edge.Item2.t_p.for_price +")  ");
                } 
      }

      
     public int policy_per_state(int index_state)
      {
        //get all nodes which are connected to main index_state node
        LinkedList<Tuple<int, State>> edgeList=ad_graph.get_linked_nodes(index_state);

        double [] pi=new double[3];
        
        foreach (Tuple<int, State> node in edgeList)
                {
                    pi[0]+= node.Item2.t_p.for_click * ( node.Item2.reward + gamma*node.Item2.state_value ) ;
                    pi[1]+= node.Item2.t_p.for_price * ( node.Item2.reward + gamma*node.Item2.state_value ) ;
                    pi[2]+= node.Item2.t_p.for_end   * ( node.Item2.reward + gamma*node.Item2.state_value ) ;
              
                } 

       double max = pi[0];
       int index=0; 
        for (int i = 0; i < pi.Length; i++) 
        { 
          if (pi[i] > max) {max = pi[i];index=i;} 
        } 
        //Console.WriteLine("pi: {0} {1} {2}", pi[0],pi[1],pi[2]);


       return   index;


       
      }

      public void  policy()
      {
        //putting max action to best_action
        for(int i=0;i<no_states;i++)
        {
         ad_graph.get_node(i).set_best_action(policy_per_state(i));
        } 
      }


      public void value_iteration()
      {
        for(int i=0;i<no_states;i++)
        {

          //get main node for every iteration
          State main_node=ad_graph.get_node(i);

          LinkedList<Tuple<int, State>> edgeList=ad_graph.get_linked_nodes(i); 
          double value_=0;
          foreach (Tuple<int, State> node in edgeList)
                {
                  int node_index=node.Item1;

                  if (policy_per_state(node_index)==0)
                    {//Console.WriteLine("act 1 {0}",value_);
                    value_+=node.Item2.t_p.for_click * ( node.Item2.reward + gamma*node.Item2.state_value ) ;}
                  else if (policy_per_state(node_index)==1)  
                    {//Console.WriteLine("act 2");
                    value_+=node.Item2.t_p.for_price * ( node.Item2.reward + gamma*node.Item2.state_value ) ;}
                  else if (policy_per_state(node_index)==2) 
                    {//Console.WriteLine("act 3");
                    value_+=node.Item2.t_p.for_end * ( node.Item2.reward + gamma*node.Item2.state_value ) ;}  
                }


          //Console.WriteLine("act 1 {0}",value_);
          main_node.set_state_value(value_);      
          //get_node(i).set_state_value(value_); //not working
          Console.WriteLine("node info {0} {1}",main_node.reward,main_node.state_value);
          Console.WriteLine("act 1 {0} {1}",value_,main_node.get_state_value());     

        }

        //Console.WriteLine(main_node.state_value);    
      }

   }


   class Main_Model
   {
      static void Main(string[] args)
      {
         
         
        MDP mdp=new MDP();
        
        int a =mdp.policy_per_state(5);
        Console.WriteLine("pi: {0} ",  a);
        Console.WriteLine("pi: {0} ",  mdp.ad_graph.get_node(10).best_action);
        
        for (int i=0;i<100;i++)
            {
              Console.WriteLine("{0} ",  i);
              mdp.policy();
              for (int j=0;j<1000;j++)
              {
                Console.WriteLine("value_iteration {0} ",  j);
                mdp.value_iteration();
              }
            }
            
         //adjacencyList.printAdjacencyList();
         //State s0=new State(.17,.2,.11);
         
         //Console.WriteLine("Reward: {0} for_price :{1}",  s0.reward,s0.t_p.for_price);
        
      }
   }
}