#include <iostream>
#include <cstdlib>
#include <vector>
#include <stdlib.h>
#include <time.h>
using namespace std;
 
typedef float state_type; 
typedef float action_type;
//features for state of MDP 

struct transition_prob
{
    action_type for_click;
    action_type for_price;
    action_type for_end;
};

struct features
 {
    state_type  pclick;
    state_type relevance;
    state_type bid;
    
    state_type reward;
    
    int best_action;  //this will be used for policy pi(s)-> a
    
    float state_value; //this will be used ofr storing the value of a state

    struct transition_prob t_p;

    features(state_type pclick, state_type relevance,state_type bid): pclick(pclick), relevance(relevance),bid(bid) {}

    state_type* get_feature()
        {   
            state_type feature_vector []={pclick,relevance,bid};
            return feature_vector;
            
        }   
    
    void set_reward()
    {   //some random way
        reward=pclick*.8+relevance*.1+bid*.1;
    }

    void set_transition_prob()
    {
        t_p.for_click=.8*pclick;
        t_p.for_price=1- pclick;
        t_p.for_end=.2*pclick;
    }
}; 

struct AdjListNode
{
    struct features *feature_vector;
    struct AdjListNode* next;
    int state_id;
};
 
// Adjacency List  
struct AdjList
{
    struct AdjListNode *head;
};
 
// Class Graph
class Graph
{
    private:
        int V;
        struct AdjList* array;
    public:
        Graph(int V)
        {
            this->V = V;
            array = new AdjList [V];
            for (int i = 0; i < V; ++i)
                array[i].head = NULL;
        }
        //Creating New Adjacency List Node
        AdjListNode* newAdjListNode(int state_id ,features* feature_vector)
        {
            AdjListNode* newNode = new AdjListNode;
            newNode->state_id=state_id;
            newNode->feature_vector=feature_vector;

            //set reward based on feature parameter
            newNode->feature_vector->set_reward();
            newNode->feature_vector->set_transition_prob();
            newNode->next = NULL;
            return newNode;
        }
        //Adding Edge to unidirectionalGraph
        void addEdge(int state_id_root,int state_id_leaf, features* feature_vector)
        {
            AdjListNode* newNode = newAdjListNode(state_id_leaf,feature_vector);
            newNode->next = array[state_id_root].head;
            array[state_id_root].head = newNode;
            
        }
        
        //Print the graph
        void printGraph()
        {
        
            for (int v = 1; v <= V; ++v)
            {
                AdjListNode* pCrawl = array[v].head;
                cout<<"\n Add vertex "<<v<<"\n";
                while (pCrawl)
                {
                    cout<<"-> "<<pCrawl->state_id<<"("<<pCrawl->feature_vector->pclick<<","<<pCrawl->feature_vector->relevance<<","<<pCrawl->feature_vector->bid<<")";
                    //cout<<"("<<pCrawl->feature_vector->reward<<","<<pCrawl->feature_vector->t_p.for_price<<") " ; //these are showing reward and transition prob
                    //state=pCrawl->feature_vector->get_feature()<<" "; //these are state values 
                    pCrawl = pCrawl->next;
                }
                cout<<endl;
            }
        }
};


// Class MDP
class MDP
{
    private:
        int V;
        struct AdjList* array;
    public:
        Graph(int V)
        {
            this->V = V;
            array = new AdjList [V];
            for (int i = 0; i < V; ++i)
                array[i].head = NULL;
        }
        
};
 
//main function 
int main()
{
    int no_states=100;
    srand (time(0));

    Graph gh(no_states);
    features *feature_vector;
    state_type relevance,bid,pclick;

    for(int i=1;i<=100;i++)
        {   
            for (int j=1;j<=100;j++)
                {   
                    pclick = state_type(rand() % 100 + 1)/100;
                    relevance= state_type(rand() % 100 + 1)/100;
                    bid=state_type(rand() % 100 + 1)/100;
                    feature_vector=new features(pclick,relevance,bid);
                    gh.addEdge(i, j,feature_vector);
                }    
        }
    
   
    gh.printGraph();
 



    return 0;
}