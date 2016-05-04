using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleGraph
{
    //CREATED BY DREAMTEAM
    //DATE: 15.03.2016
    
    /// <summary>
    /// Simple representation of graph
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    public class Graph<T>
    {   
        /// <summary>
        /// All nodes of graph
        /// </summary>
        public readonly List<T> Nodes;
        /// <summary>
        /// All edges of graph
        /// </summary>
        public readonly List<Link<T>> Edges;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Graph()
        {
            Nodes = new List<T>();
            Edges = new List<Link<T>>();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="Nodes">List with graph's items</param>
        /// <param name="Edges">List with graph's edges</param>
        public Graph(List<T> Nodes, List<Link<T>> Edges)
        {
            //Check our arguments for NULL
            if (Nodes == null || Edges == null)
                throw new ArgumentNullException();

            this.Nodes = new List<T>();
            //Insert our nodes and remove duplicate
            this.Nodes.AddRange(Nodes.Distinct().ToList());

            this.Edges = new List<Link<T>>(Edges);
            // Remove all edges wich don't have their own nodes.
            this.Edges.RemoveAll(e => !this.Nodes.Contains(e.Child) || !this.Nodes.Contains(e.Parent));
            //Also, remove duplicate
            this.Edges = this.Edges.Distinct().ToList();
        }

        /// <summary>
        /// Generate adjaceny matrix
        /// </summary>
        public bool[,] GetAdjacenyMatrix()
        {
            bool[,] AdjacentMatrix = new bool[Nodes.Count, Nodes.Count];
            //Fill matrix for each edge
            foreach (Link<T> link in Edges)
            {
                AdjacentMatrix[Nodes.IndexOf(link.Parent), Nodes.IndexOf(link.Child)] = true;
            }
            return AdjacentMatrix;
        }

        /// <summary>
        /// Add the node
        /// </summary>
        /// <param name="node">Node</param>
        public void AddNode(T node)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            Nodes.Add(node);
            //Remove duplicate
            Nodes.Distinct();
        }
        
        /// <summary>
        /// Remove required node
        /// </summary>
        /// <param name="node">Graph node</param>
        public void RemoveNode(T node)
        {
            //Remove node
            Nodes.Remove(node);
            //Remve all links for qurrent node
            Edges.RemoveAll(s => (s.Child.Equals(node) || s.Parent.Equals(node)));
        }

        /// <summary>
        /// Add the edge
        /// </summary>
        /// <param name="edge">Edge</param>
        public void AddEdge(Link<T> edge)
        {
            if (edge == null)
                throw new ArgumentNullException("edge");
            //Check exists parent and children node
            if (Nodes.Contains(edge.Parent) && Nodes.Contains(edge.Child))
            {
                Edges.Add(edge);
                //Remove duplicate
                Edges.Distinct();
            }
        }

        /// <summary>
        /// Remove reuired edge
        /// </summary>
        /// <param name="edge">Required edge</param>
        public void RemoveEdge(Link<T> edge)
        {
            //It's simple. Isn't it?
            Edges.Remove(edge);
        }

        /// <summary>
        /// Apply relations for our graph
        /// </summary>
        /// <param name="AdjacenyMatrix">Adjacent matrix with links for graph.
        /// Size have to be [n, n], where { n } is count of elements in graph.</param>
        public void SetEdges(bool[,] AdjacenyMatrix)
        {
            //Check matrix is correct
            if (AdjacenyMatrix == null)
                throw new ArgumentNullException("AdjacentMatrix");
            if (AdjacenyMatrix.GetLength(0) != AdjacenyMatrix.GetLength(1))
                throw new ArgumentException("The adjacency matrix must be square.", "AdjacentMatrix");
            if (AdjacenyMatrix.GetLength(0) > Nodes.Count)
                throw new ArgumentException("The number of connecting vertices exceeds the number of vertices", "AdjacentMatrix");

            //Clear last set of edges...
            Edges.Clear();

            // ...and generate new links!
            for (int i = 0; i < AdjacenyMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < AdjacenyMatrix.GetLength(1); j++)
                {
                    if (AdjacenyMatrix[i, j])
                    {
                        Link<T> edge = new Link<T>(Nodes[i], Nodes[j]);
                        Edges.Add(edge);
                    }
                }
            }
        }
            
        /// <summary>
        /// Union graph
        /// </summary>
        /// <param name="first">Original graph</param>
        /// <param name="second">Original graph</param>
        /// <returns>United graph</returns>
        public static Graph<T> Union(Graph<T> first, Graph<T> second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException();

            //Union edges
            List<Link<T>> links = new List<Link<T>>(first.Edges);
            links.AddRange(second.Edges);

            //And union nodes
            List<T> Nodes = new List<T>(first.Nodes);
            Nodes.AddRange(second.Nodes);

            //As result, we have united graph
            return new Graph<T>(Nodes, links);
        }

        /// <summary>
        /// Find graph's intersection
        /// </summary>
        /// <param name="first">Original graph</param>
        /// <param name="second">Original graph</param>
        /// <returns>Intersection of graphs</returns>
        public static Graph<T> Intersection(Graph<T> first, Graph<T> second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException();
            
            //Intersect nodes and edges and return new graph
            return new Graph<T>(first.Nodes.Intersect(second.Nodes).ToList(), first.Edges.Intersect(second.Edges).ToList());
        }

        /// <summary>
        /// Graph difference
        /// </summary>
        /// <param name="first">Graph</param>
        /// <param name="second">Graph</param>
        /// <returns>Graph, based on difference bunches of nodes deducted graphs</returns>
        public static Graph<T> Difference(Graph<T> first, Graph<T> second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException();

            List<T> nodes = new List<T>(first.Nodes);
            //Remove all nodes from first graph that contained in the second graph
            nodes.RemoveAll(s => second.Nodes.Contains(s));

            List<Link<T>> edges = new List<Link<T>>(first.Edges);

            //Return result graph
            return new Graph<T>(nodes, edges);
        }

        /// <summary>
        /// Ring sum of graphs
        /// </summary>
        /// <param name="first">First graph</param>
        /// <param name="second">Second graph</param>
        /// <returns>Sum of graphs</returns>
        public static Graph<T> RingSum(Graph<T> first, Graph<T> second)
        {
            List<Link<T>> firstEdges = new List<Link<T>>(first.Edges);
            List<Link<T>> secondEdges = new List<Link<T>>(second.Edges);

            //Remove equal edges from each graph
            firstEdges.RemoveAll(s => second.Edges.Contains(s));
            secondEdges.RemoveAll(s => first.Edges.Contains(s));

            List<Link<T>> edges = new List<Link<T>>(firstEdges);
            edges.AddRange(secondEdges);
            List<T> nodes = new List<T>(first.Nodes);
            nodes.AddRange(second.Nodes);
            //Return new graph
            return new Graph<T>(nodes, edges);
        }
        
        /// <summary>
        /// Intersection of two sets
        /// </summary>
        /// <param name="a">First set</param>
        /// <param name="b">Second set</param>
        /// <returns></returns>
        public static List<T> Intersection(List<T> a, List<T> b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException();

            //The intersection of two sets
            return a.Intersect(b).ToList();
        }

        /// <summary>
        /// Direct mapping
        /// </summary>
        /// <param name="node">Base graph element</param>
        /// <returns>Node graph set, that is dirrect map</returns>
        public List<T> DirectMapping(T node)
        {
            //Find all nodes, that are children for specified node
            List<Link<T>> edges = Edges.FindAll(e => e.Parent.Equals(node));
            List<T> result = new List<T>();
            foreach(Link<T> edge in edges)
            {
                result.Add(edge.Child);
            }

            return result;
        }

        /// <summary>
        /// Direct mapping
        /// </summary>
        /// <param name="nodes">Base graph elements</param>
        /// <returns>Node graph set, that is dirrect map</returns>
        public List<T> DirectMapping(List<T> nodes)
        {
            //Find all nodes that are childrens for each specified node
            List<T> result = new List<T>();
            foreach(T node in nodes)
            {
                result.AddRange(DirectMapping(node));
            }
            return result.Distinct().ToList();
        }

        /// <summary>
        /// Inverse mapping
        /// </summary>
        /// <param name="node">Base graph element</param>
        /// <returns>Node graph set, that is inverse map</returns>
        public List<T> InverseMapping(T node)
        {
            //Find all nodes, that are parents for specified node
            List<Link<T>> edges = Edges.FindAll(e => e.Child.Equals(node));
            List<T> result = new List<T>();
            foreach (Link<T> edge in edges)
            {
                result.Add(edge.Parent);
            }
            return result;
        }

        /// <summary>
        /// Inverse mapping
        /// </summary>
        /// <param name="nodes">Base graph elements</param>
        /// <returns>Node graph set, that is inverse map</returns>
        public List<T> InverseMapping(List<T> nodes)
        {
            //Find all nodes, that are parent for each specified nodes
            List<T> result = new List<T>();
            foreach (T node in nodes)
            {
                result.AddRange(InverseMapping(node));
            }
            return result.Distinct().ToList();
        }

        /// <summary>
        /// Direct transitive closure
        /// </summary>
        /// <param name="node">Closure node</param>
        /// <returns>Set of transitive closure's nodes</returns>
        public List<T> DirectTransitiveClosure(T node)
        {
            int lastSize = 0;
            List<T> closure = new List<T>();
            closure.Add(node);
            while (lastSize != closure.Count)
            {
                //Find direct mapping for each node and after find mapping for each founded node.
                //If size of nodes remains fixed, return found nodes.
                lastSize = closure.Count;
                closure.AddRange(DirectMapping(closure));
                closure = closure.Distinct().ToList();
            }
            return closure;
        }

        /// <summary>
        /// Inverse transitive closure
        /// </summary>
        /// <param name="node">Closure node</param>
        /// <returns>Set of transitive closure's nodes</returns>
        public List<T> InverseTransitiveClosure(T node)
        {
            int lastSize = 0;
            List<T> closure = new List<T>();
            closure.Add(node);
            while (lastSize != closure.Count)
            {
                //Find inverse mapping for each node and after find mapping for each founded node.
                //If size of nodes remains fixed, return found nodes.
                lastSize = closure.Count;
                closure.AddRange(InverseMapping(closure));
                closure = closure.Distinct().ToList();
            }
            return closure;
        }

        /// <summary>
        /// Calculate attainbility matrix
        /// </summary>
        /// <returns>Attainbility matrix</returns>
        public bool[,] GetAttainbilityMatrix()
        {
            bool[,] mat = new bool[Nodes.Count, Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                //Find all nodes, that have path from selected node.
                List<T> accessibleNodes = DirectTransitiveClosure(Nodes[i]);
                foreach(T node in accessibleNodes)
                {
                    mat[i, Nodes.IndexOf(node)] = true;
                }
            }
            return mat;
        }

        /// <summary>
        /// Calculate inaccessibility matrix
        /// </summary>
        /// <returns>Inaccessibility matrix</returns>
        public bool[,] GetInaccessibilityMatrix()
        {
            bool[,] mat = new bool[Nodes.Count, Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                //Find all path that exist to selected node
                List<T> accessibleNodes = InverseTransitiveClosure(Nodes[i]);
                foreach (T node in accessibleNodes)
                {
                    mat[i, Nodes.IndexOf(node)] = true;
                }
            }
            return mat;
        }

        /// <summary>
        /// Split graph to highly connected subgraphs
        /// </summary>
        /// <returns>Splited graph</returns>
        public Graph<Graph<T>> MalgrangePartion()
        {
            //Split graph
            List<Graph<T>> subGraphs = new List<Graph<T>>();
            Graph<T> procGraph = this;
            //For each node try to find their subgraph
            while(procGraph.Nodes.Count != 0)
            {
                List<T> dirClosure = DirectTransitiveClosure(procGraph.Nodes[0]);
                List<T> invClosure = InverseTransitiveClosure(procGraph.Nodes[0]);

                //Create strong subgraph, that based on intersection of direct closure and inverse closure, set of edges
                Graph<T> strongSubgraph = new Graph<T>(Intersection(dirClosure, invClosure), Edges);
                subGraphs.Add(strongSubgraph);

                procGraph = Difference(procGraph, strongSubgraph);
            }

            //Claculate condensations
            List<Link<Graph<T>>> edges = new List<Link<Graph<T>>>();
            foreach(Graph<T> graphPar in subGraphs)
            {
                //It's really hard for perception
                //Long story short: find all subgraph, that have at least one sub edge
                List<Graph<T>> n = subGraphs.FindAll(s => Edges.Exists(e => graphPar.Nodes.Contains(e.Parent) && s.Nodes.Contains(e.Child) && !s.Equals(graphPar)));
                foreach(Graph<T> graphChild in n)
                {
                    Link<Graph<T>> edge = new Link<Graph<T>>(graphPar, graphChild);
                    edges.Add(edge);
                }
            }

            return new Graph<Graph<T>>(subGraphs, edges);
        }
    } 
}
