using System;
using System.Collections.Generic;
using System.Linq;
namespace Ad_graph
{

   
   struct State
   {  

      public int state_no;

      private double  pclick, relevance, bid;
    
      public  double reward;
    
      public int best_action;  //this will be used for policy pi->a
    
      public double state_value; //this will be used ofr storing the value of a state

      public double [] for_click,for_price,for_end;
      
      public int no_nodes_linked;

      public State(double pclick_, double relevance_,double bid_,int state_no,int no_nodes_linked):this()
         {
            this.pclick=pclick_; 
            this.relevance=relevance_;
            this.bid=bid_; 

            this.state_no=state_no;
            this.no_nodes_linked=no_nodes_linked;
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

        for_price=new double[no_nodes_linked];
        for_end=new double[no_nodes_linked];
        for_click=new double[no_nodes_linked];


        for (int i=0;i<no_nodes_linked;i++)
        {
         for_click[i]= .8*pclick;
         for_price[i]=1- pclick;
         for_end[i]=.2*pclick;
       }
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

      public int get_best_action()
      {
        return best_action;
      }
   };//end of struct feature 
   
   

   
   class AdjacencyList
    {
        public LinkedList<State>[] adjacencyList;
        
        // Constructor - creates an empty Adjacency List
        public AdjacencyList(int vertices)
        {
            adjacencyList = new LinkedList<State>[vertices];

 
            for (int i = 0; i < adjacencyList.Length; ++i)
            {
                adjacencyList[i] = new LinkedList<State>();
            }
        }
 
        // Appends a new Edge to the linked list
        public void addEdgeAtEnd(int startVertex, State s)
        {
            adjacencyList[startVertex].AddLast(s);
        }
 
        // Adds a new Edge to the linked list from the front
        public void addEdgeAtBegin(int startVertex, State s)
        {
            adjacencyList[startVertex].AddFirst(s);
        }
 
 
        // Returns a copy of the Linked List of outward edges from a vertex
         public LinkedList<State> get_linked_nodes(int index)
         {
            
            LinkedList<State> edgeList
                               = new LinkedList<State>(adjacencyList[index]);
            return edgeList;
         }
 

        // this is wrong implementation
        public State get_node(int index)
        { 
          LinkedList<State> list=adjacencyList[index];
          State node;
          for(int i =0;i<index;i++)
              node=list[i];
          return node;    
          //return adjacencyList[index].ElementAt(index);
        }      

        // Prints the Adjacency List
        public void printAdjacencyList()
        {
            int i = 0;
 
            foreach (LinkedList<State> list in adjacencyList)
            {
                Console.Write("adjacencyList[" + i + "] -> ");
 
                foreach (State edge in list)
                {
                    Console.Write("(" + edge.reward + " "+edge.for_price +")  ");
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
                    ad_graph.addEdgeAtEnd(i,new State(pclick,relevance,bid,j,no_states));
                }    
         }

         return ad_graph;
      }

      public void  show_adjacent_nodes(int index)
      {
          LinkedList<State> edgeList=ad_graph.get_linked_nodes(index);
      
          foreach (State edge in edgeList)
                {
                    //Console.Write("(" + edge.reward + " "+edge.t_p.for_price +")  ");
                  Console.Write("("   + edge.get_best_action() + " "+edge.get_state_value()+")  ");
                } 
      }

      
     public int policy_per_state(int index_state)
      {
        //get all nodes which are connected to main index_state node
        LinkedList<State> edgeList=ad_graph.get_linked_nodes(index_state);

        State main_node=ad_graph.get_node(index_state);

        double [] pi=new double[3];
        
        foreach ( State node in edgeList)
                {
                    int index=node.state_no;
                    pi[0]+= main_node.for_click[index] * ( node.reward + gamma*node.state_value ) ;
                    pi[1]+= main_node.for_price[index] * ( node.reward + gamma*node.state_value ) ;
                    pi[2]+= main_node.for_end[index]   * ( node.reward + gamma*node.state_value ) ;
              
                } 

       
        double max = pi[0];
        int pos=0; 
        for (int i = 0; i < pi.Length; i++) 
          { 
            if (pi[i] > max) {max = pi[i];pos=i;} 
          } 
        //Console.WriteLine("pi: {0} {1} {2}", pi[0],pi[1],pi[2]);


        return   pos;
      
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
          //get main_node 
          State main_node=ad_graph.get_node(i);
          // State main_node=ad_graph.adjacencyList[i].ElementAt(i);

          int main_node_index=main_node.state_no; // this is also equal to i 
          int best_action=policy_per_state(main_node_index);

          //Console.WriteLine("({0} {1} )",main_node.state_no,i); // to check if the get_node is giving correct node 

          //get the linked nodes to main_node
          LinkedList<State> edgeList=ad_graph.get_linked_nodes(i); 
          double value_=0;

          //calculate utility for each node
          foreach (State node in edgeList)
                {
                  int node_linked_index=node.state_no;
                  

                  if (best_action==0)
                    { 
                      value_+=main_node.for_click[node_linked_index] * ( node.reward + gamma*node.state_value ) ;
                    }

                  else if (best_action==1)  
                    { 
                      value_+=main_node.for_price[node_linked_index] * ( node.reward + gamma*node.state_value ) ;
                    }

                  else if (best_action==2) 
                    { 
                      value_+=main_node.for_end[node_linked_index] * ( node.reward + gamma*node.state_value ) ;
                    }

                } //inner for loop ends

          main_node.set_state_value(value_);     // here we  put utility in state node  
          //ad_graph.adjacencyList[i].ElementAt(i).set_state_value(value_);
          Console.WriteLine("VI {0} {1} {2}",i ,main_node.get_state_value(),ad_graph.adjacencyList[i].ElementAt(i).get_state_value()); 
          
        }// outer for loop ends

         //Console.WriteLine("VI {0} ",ad_graph.get_node(55).get_state_value()); 
       // Console.WriteLine(main_node.state_value);    
        
        
      }


      public void Bellmen_convergence()
      {
        for (int i=0;i<no_states;i++)
            {
             
              policy();

              for (int j=0;j<100;j++)
              {
                value_iteration();
              }

              for(int l=0;l<100;l++)
                   //mdp.show_adjacent_nodes(l);
                  Console.WriteLine("{0} ",ad_graph.get_node(l).get_state_value());
            }
        
      }

   }


   class Main_Model
   {
      static void Main(string[] args)
      {
         
        MDP mdp=new MDP();

        mdp.Bellmen_convergence();
         
      }
   }
}