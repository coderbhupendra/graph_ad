// Program to print BFS traversal from a given source vertex. BFS(int s) 
// traverses vertices reachable from s.
#include<iostream>
#include <list>

using namespace std;

struct state_feature_vector
{
	float pclick;
	float relevance;
};

// This class represents a directed graph using adjacency list representation
class Graph
{
    int V;    // No. of vertices
    list<state_feature_vector> *adj;    // Pointer to an array containing adjacency lists
public:
    Graph(int V);  // Constructor
    void addEdge(state_feature_vector  centre_state, state_feature_vector  state); // function to add an edge to graph
    void BFS(state_feature_vector state);  // prints BFS traversal from a given source s
};

Graph::Graph(int V)
{
    this->V = V;
    adj = new list<state_feature_vector>[V];
}

void Graph::addEdge(state_feature_vector centre_state, state_feature_vector state)
{
    adj[centre_state].push_back(state); // Add w to v’s list.
}

void Graph::BFS(struct state_feature_vector state)
{
    // Mark all the vertices as not visited
    bool *visited = new bool[V];
    for(int i = 0; i < V; i++)
        visited[i] = false;

    // Create a queue for BFS
    list<struct state_feature_vector> queue;

    // Mark the current node as visited and enqueue it
    visited[state] = true;
    queue.push_back(state);

    // 'i' will be used to get all adjacent vertices of a vertex
    list<struct state_feature_vector>::iterator i;

    while(!queue.empty())
    {
        // Dequeue a vertex from queue and print it
        state = queue.front();
        cout << state.pclick << " ";
        queue.pop_front();

        // Get all adjacent vertices of the dequeued vertex s
        // If a adjacent has not been visited, then mark it visited
        // and enqueue it
        for(i = adj[state].begin(); i != adj[state].end(); ++i)
        {
            if(!visited[*i])
            {
                visited[*i] = true;
                queue.push_back(*i);
            }
        }
    }
}

// Driver program to test methods of graph class
int main()
{
    // Create a graph given in the above diagram
    Graph g(4);
    g.addEdge({70.0, 1.0}, {72.0, 1.0});
    g.addEdge({71.0, 1.0},  {73.0, 1.0});
    g.addEdge({70.0, 1.0},  {72.0, 1.0});
    g.addEdge({72.0, 1.0},  {70.0, 1.0});
    g.addEdge({70.0, 1.0},  {71.0, 1.0});
    g.addEdge({72.0, 1.0},  {70.0, 1.0});

    cout << "Following is Breadth First Traversal (starting from vertex 2) \n";
    g.BFS({70.0, 1.0});

    return 0;
}