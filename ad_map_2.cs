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
         for_click[i]=1.0/no_nodes_linked; //.8*pclick;
         for_price[i]=1.0/no_nodes_linked;//1- pclick;
         for_end[i]=1.0/no_nodes_linked;//.2*pclick;
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
        public List<State>[] adjacencyList;
        
        // Constructor - creates an empty Adjacency List
        public AdjacencyList(int vertices)
        {
            adjacencyList = new List<State>[vertices];

 
            for (int i = 0; i < adjacencyList.Length; ++i)
            {
                adjacencyList[i] = new List<State>();
            }
        }
 
        // Appends a new Edge to the linked list
        public void addEdge(int startVertex, State s)
        {
            adjacencyList[startVertex].Add(s);
        }
 
       
 
        // Returns a copy of the Linked List of outward edges from a vertex
         public List<State> get_linked_nodes(int index)
         {
            
            List<State> edgeList = new List<State>(adjacencyList[index]);
            return edgeList;
         }
 

        // this is wrong implementation
        public State get_node(int list_index,int node_index)
        { 
          
          return adjacencyList[list_index][node_index];
        }      

        // Prints the Adjacency List
        public void printAdjacencyList()
        {
            int i = 0;
 
            foreach (List<State> list in adjacencyList)
            {
                Console.Write("adjacencyList[" + i + "] -> ");
 
                foreach (State edge in list)
                {
                    Console.Write("(" + edge.reward + " "+edge.for_price +")  ");
                }
 
                ++i;
                
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
                    ad_graph.addEdge(i,new State(pclick,relevance,bid,j,no_states));
                }    
         }

         return ad_graph;
      }

      
      
     public int policy_per_state(int index_state)
      {
        //get all nodes which are connected to main index_state node
        List<State> edgeList=ad_graph.get_linked_nodes(index_state);

        State main_node=ad_graph.get_node(index_state,index_state);

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
        //calculating best action for all states.
        for(int i=0;i<no_states;i++)
        {

         
         int best_action=policy_per_state(i);

         for(int k =0;k<no_states;k++)
            {
              State temp_node=ad_graph.get_node(k,i);
              temp_node.set_best_action(best_action);
              ad_graph.adjacencyList[k][i]=temp_node; 
            }
         
        } 
      }


      public void value_iteration()
      {
        
        for(int i=0;i<no_states;i++)
        {
          //get main_node 
          State main_node=ad_graph.get_node(i,i);
           
          int main_node_index=main_node.state_no; // this is also equal to i 
          int best_action=policy_per_state(main_node_index);

         
          //get the linked nodes to main_node
          List<State> edgeList=ad_graph.get_linked_nodes(i); 
          double value_=0;

          //calculate utility for each node
          foreach (State node in edgeList)
                {
                  int node_linked_index=node.state_no;
                  

                  if (best_action==0)
                    { //Console.WriteLine("{0} {1} {2} {3} {4}",0,node.reward,node.state_value,node.state_no,main_node_index);
                      value_+=main_node.for_click[node_linked_index] * ( node.reward + gamma*node.state_value ) ;
                    }

                  else if (best_action==1)  
                    {  //Console.WriteLine("{0} {1} {2} {3} {4}",1,node.reward,node.state_value,node.state_no,main_node_index);
                      value_+=main_node.for_price[node_linked_index] * ( node.reward + gamma*node.state_value ) ;
                    }

                  else if (best_action==2) 
                    {  //Console.WriteLine("{0} {1} {2} {3} {4}",2,node.reward,node.state_value,node.state_no,main_node_index);
                      value_+=main_node.for_end[node_linked_index] * ( node.reward + gamma*node.state_value ) ;
                    }

                } //inner for loop ends

          main_node.set_state_value(value_);     // here we  put utility in state node  
          
          ad_graph.adjacencyList[i][i]=main_node;

          for(int k =0;k<no_states;k++)
            {
              State temp_node=ad_graph.get_node(k,main_node_index);
              temp_node.set_state_value(value_);
              ad_graph.adjacencyList[k][main_node_index]=temp_node; 
            }
          Console.WriteLine("VI {0} {1} ",i ,main_node.get_state_value(),ad_graph.adjacencyList[i][i].get_state_value()); 
          
        }// outer for loop ends

        
        
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
              //Console.WriteLine("VI {0} ",ad_graph.get_node(55,55).get_state_value()); 
       
              //Console.WriteLine("{0}",ad_graph.get_node(55,55).get_best_action());
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