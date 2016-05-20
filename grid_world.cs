using System;
using System.Collections.Generic;
using System.Linq;
namespace Ad_graph
{

   struct State
   {  

      public int state_no;

    
      public  double reward;
    
      public int best_action;  //this will be used for policy pi->a
    
      public double state_value; //this will be used for storing the value of a state

      
      
      public  double[,] transition_prob;
      
      
      
      public int no_nodes_linked;

      public State(double reward ,int state_no,int no_nodes_linked):this()
         {
             

            this.state_no=state_no;
            this.no_nodes_linked=no_nodes_linked;
            this.reward=reward;

            //calulating the reward and transition probability
          
            set_transition_prob();
         } 

       
    
     

      public void set_transition_prob()
      {
        int no_action=16;
        transition_prob=new double[no_action,no_nodes_linked];
       
        for (int i=0;i<no_action;i++)
        {
          for (int j=0;j<no_nodes_linked;j++)
          {
            transition_prob[i,j]= 1/no_nodes_linked;
          }
        
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
        public List<int>[] adjacencyList;

        
        // Constructor - creates an empty Adjacency List
        public AdjacencyList(int vertices)
        {
            adjacencyList = new List<int>[vertices];

            for (int i = 0; i < adjacencyList.Length; ++i)
            {
                adjacencyList[i] = new List<int>();
            }
        }
 
        // Appends a new Edge to the linked list
        public void addEdge(int start_node, int end_node )
        {
            adjacencyList[start_node].Add(end_node);
        }
 
       
 
        // Returns a copy of the Linked List of outward edges from a vertex
         public List<int> get_linked_nodes(int index)
         {
            List<int> edgeList = new List<int>(adjacencyList[index]);
            return edgeList;
         }
 
       
  
    }
 
   class MDP 
   {

      public AdjacencyList Ad_Graph;
      public List<State> Global_nodes;
      //MDP Framework 
      int no_states =16;
      double gamma=0.9;
      int no_action=16;

      //constructor      
      public MDP()
      {
          Global_nodes=new List<State>();
          initialize_states();
          initialize_graph();   
      }

      public void initialize_states()
      {

          for (int j=0;j<no_states;j++)
                {   
                    Global_nodes.Add(new State(-.04,j,no_states));
                }  


                Global_nodes[10]=(new State(10,10,no_states));
                Global_nodes[14]=(new State(5,14,no_states));
                Global_nodes[3]=(new State(2,3,no_states));
                Global_nodes[15]=(new State(-10,15,no_states));

      }

      public void initialize_graph()
      {
          Ad_Graph = new AdjacencyList(no_states); 
        
        //do the graph connection
          for(int i=0;i<no_states;i++)
          {   
            for (int j=0;j<no_states;j++)
                {   
                    Ad_Graph.addEdge(i,j);
                }    
          }
   
      }

      
      public int policy_per_state(int index_state)
      {
        State main_node=Global_nodes[index_state];
        //get all nodes which are connected to main index_state node
        List<int> edgeList=Ad_Graph.get_linked_nodes(index_state);

        double [] pi=new double[3];
        
        foreach ( int node_no in edgeList)
                { 
                    State connected_node=Global_nodes[node_no];
                    double connected_node_value=connected_node.state_value;
                    int connected_node_index=connected_node.state_no; //=node_no 

                    for (int j=0;j<no_action;j++)
                      pi[j]+= main_node.transition_prob[j,connected_node_index] *connected_node_value;

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
          State temp_node=Global_nodes[i];
          temp_node.set_best_action(best_action);
          Global_nodes[i]=temp_node; 
        }
         
      }


      public void value_iteration()
      {
        
        for(int i=0;i<no_states;i++)
        {
          //get main_node 
          State main_node=Global_nodes[i];
           
          int main_node_index=main_node.state_no; // this is also equal to i 
          double main_node_reward=main_node.reward;
          int best_action=policy_per_state(main_node_index);

         
          //get the linked nodes to main_node
          List<int> edgeList=Ad_Graph.get_linked_nodes(i); 
          double value_=0;

          //calculate utility for each node
          foreach (int node_no in edgeList)
                { 
                  State connected_node=Global_nodes[node_no];
                  double connected_node_value=connected_node.state_value;
                  int connected_node_index=connected_node.state_no; //=node_no 
                  value_+=main_node.transition_prob[best_action,connected_node_index] * connected_node_value  ;
                  
                } //inner for loop ends

          value_=gamma*value_ + main_node_reward;      
          main_node.set_state_value(value_);     // here we  put utility in state node  
          
          Global_nodes[main_node_index]=main_node;

          //Console.WriteLine("VI {0} {1} {2} ",i ,main_node.get_state_value(),Global_nodes[main_node_index].get_state_value()); 
          
        }// outer for loop ends

      }


      public void Bellmen_convergence()
      {
        learned_values();
        for (int i=0;i<no_states;i++)
            {
             
              policy();

              for (int j=0;j<100;j++)
              {
                value_iteration();
              }
              learned_values();
            }
        
        learned_values();
      }

      public void learned_values()
      {
        for (int i=0;i<no_states;i++)
        {
           Console.Write("{0} {1} {2}",Global_nodes[i].get_state_value(),Global_nodes[i].get_best_action(),Global_nodes[i].reward); 
        }
       
      }

   }


   class Main_Model
   {  

      //public Static List<State> Global_nodes= new List<State>;

      
      static void Main(string[] args)
      {
           
        MDP mdp=new MDP();

        mdp.Bellmen_convergence();
         
      }
   }
} 