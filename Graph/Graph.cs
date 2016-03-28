using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleGraph
{
    //CREATED BY ANDREY SOKOLOV
    //DATE: 15.03.2016
    
    /// <summary>
    /// Граф
    /// </summary>
    /// <typeparam name="T">Тип элементов графа</typeparam>
    public class Graph<T>
    {   
        /// <summary>
        /// Все узлы графа
        /// </summary>
        public readonly List<T> Nodes;
        /// <summary>
        /// Все связи графа
        /// </summary>
        public readonly List<Link<T>> Edges;

        /// <summary>
        /// Создать новый граф
        /// </summary>
        /// <param name="Edges">Описание связей</param>
        public Graph(List<T> Nodes, List<Link<T>> Edges)
        {
            if (Nodes == null || Edges == null)
                throw new ArgumentNullException();

            this.Nodes = new List<T>();
            this.Nodes.AddRange(Nodes.Distinct().ToList());

            this.Edges = new List<Link<T>>(Edges);

            // Удаляем связи для которых не существует вершин
            this.Edges.RemoveAll(e => !this.Nodes.Contains(e.Child) || !this.Nodes.Contains(e.Parent));
            this.Edges = this.Edges.Distinct().ToList();
        }

        /// <summary>
        /// Сгенерировать матрицу смежности
        /// </summary>
        public bool[,] GetAdjacenyMatrix()
        {
            bool[,] AdjacentMatrix = new bool[Nodes.Count, Nodes.Count]; 
            foreach (Link<T> link in Edges)
            {
                AdjacentMatrix[Nodes.IndexOf(link.Parent), Nodes.IndexOf(link.Child)] = true;
            }
            return AdjacentMatrix;
        }
        
        /// <summary>
        /// Удалить указанную вершину
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(T node)
        {
            Nodes.Remove(node);
            Edges.RemoveAll(s => (s.Child.Equals(node) || s.Parent.Equals(node)));
        }

        /// <summary>
        /// Удалить ребро
        /// </summary>
        /// <param name="edge">Удаляемое ребро</param>
        public void RemoveEdge(Link<T> edge)
        {
            Edges.Remove(edge);
        }

        /// <summary>
        /// Изменить связи в графе
        /// </summary>
        /// <param name="AdjacenyMatrix">Матрица смежности связей в графе</param>
        public void SetEdges(bool[,] AdjacenyMatrix)
        {
            if (AdjacenyMatrix == null)
                throw new ArgumentNullException("AdjacentMatrix");
            if (AdjacenyMatrix.GetLength(0) != AdjacenyMatrix.GetLength(1))
                throw new ArgumentException("The adjacency matrix must be square.", "AdjacentMatrix");
            if (AdjacenyMatrix.GetLength(0) > Nodes.Count)
                throw new ArgumentException("The number of connecting vertices exceeds the number of vertices", "AdjacentMatrix");

            Edges.Clear();

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
        /// Произвести слияние графов
        /// </summary>
        /// <param name="first">Исходный граф</param>
        /// <param name="second">Исходный граф</param>
        /// <returns>Граф слияния</returns>
        public static Graph<T> Union(Graph<T> first, Graph<T> second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException();

            List<Link<T>> links = new List<Link<T>>(first.Edges);
            links.AddRange(second.Edges);

            List<T> Nodes = new List<T>(first.Nodes);
            Nodes.AddRange(second.Nodes);

            return new Graph<T>(Nodes, links);
        }

        /// <summary>
        /// Произвести пересечение графов 
        /// </summary>
        /// <param name="first">Исходный граф</param>
        /// <param name="second">Исхожный граф</param>
        /// <returns>Граф пересечения</returns>
        public static Graph<T> Intersection(Graph<T> first, Graph<T> second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException();

            List<T> Nodes = new List<T>(first.Nodes);
            Nodes.AddRange(second.Nodes);

            return new Graph<T>(Nodes, first.Edges.Intersect(second.Edges).ToList());
        }

        /// <summary>
        /// Разность графов
        /// </summary>
        /// <param name="first">Граф</param>
        /// <param name="second">Граф</param>
        /// <returns>Граф, основанный на разности множеств вершин вычитаемых графов</returns>
        public static Graph<T> Difference(Graph<T> first, Graph<T> second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException();

            List<T> nodes = new List<T>(first.Nodes);
            nodes.RemoveAll(s => second.Nodes.Contains(s));

            List<Link<T>> edges = new List<Link<T>>(first.Edges);
            edges.AddRange(second.Edges);

            return new Graph<T>(nodes, edges);
        }

        /// <summary>
        /// Кольцевая сумма графов
        /// </summary>
        /// <param name="first">Исходный граф</param>
        /// <param name="second">Исходный граф</param>
        /// <returns>Сумма графов</returns>
        public static Graph<T> RingSum(Graph<T> first, Graph<T> second)
        {
            List<Link<T>> firstEdges = new List<Link<T>>(first.Edges);
            List<Link<T>> secondEdges = new List<Link<T>>(second.Edges);

            firstEdges.RemoveAll(s => second.Edges.Contains(s));
            secondEdges.RemoveAll(s => first.Edges.Contains(s));

            List<Link<T>> edges = new List<Link<T>>(firstEdges);
            edges.AddRange(secondEdges);
            List<T> nodes = new List<T>(first.Nodes);
            nodes.AddRange(second.Nodes);

            return new Graph<T>(nodes, edges);
        }
        
        /// <summary>
        /// Пересечение двух наборов вершин
        /// </summary>
        /// <param name="a">Набор вершин</param>
        /// <param name="b">Набор вершин</param>
        /// <returns></returns>
        public static List<T> Intersection(List<T> a, List<T> b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException();

            return a.Intersect(b).ToList();
        }

        /// <summary>
        /// Прямое отображение
        /// </summary>
        /// <param name="node">Исходный элемент графа</param>
        /// <returns>Набор вершин графа, являющийся прямым отображением</returns>
        public List<T> DirectMapping(T node)
        {
            List<Link<T>> edges = Edges.FindAll(e => e.Parent.Equals(node));
            List<T> result = new List<T>();
            foreach(Link<T> edge in edges)
            {
                result.Add(edge.Child);
            }

            return result;
        }

        /// <summary>
        /// Прямое отображение
        /// </summary>
        /// <param name="nodes">Исходные элементы графа</param>
        /// <returns>Набор вершин графа, являющийся прямым отображением</returns>
        public List<T> DirectMapping(List<T> nodes)
        {
            List<T> result = new List<T>();
            foreach(T node in nodes)
            {
                result.AddRange(DirectMapping(node));
            }
            return result.Distinct().ToList();
        }

        /// <summary>
        /// Обратное отображение
        /// </summary>
        /// <param name="node">Исходный элемент графа</param>
        /// <returns>Набор вершин графа, являющийся обратным отображением</returns>
        public List<T> InverseMapping(T node)
        {
            List<Link<T>> edges = Edges.FindAll(e => e.Child.Equals(node));
            List<T> result = new List<T>();
            foreach (Link<T> edge in edges)
            {
                result.Add(edge.Parent);
            }
            return result;
        }

        /// <summary>
        /// Обратное отображение
        /// </summary>
        /// <param name="nodes">Исходные элементы графа</param>
        /// <returns>Набор вершин графа, являющийся обратным отображением</returns>
        public List<T> InverseMapping(List<T> nodes)
        {
            List<T> result = new List<T>();
            foreach (T node in nodes)
            {
                result.AddRange(InverseMapping(node));
            }
            return result.Distinct().ToList();
        }

        /// <summary>
        /// Прямое транзитивное замыкание
        /// </summary>
        /// <param name="node">Вершина замыкания</param>
        /// <returns>Набор вершин прямого транзитивного замыкания</returns>
        public List<T> DirectTransitiveClosure(T node)
        {
            int lastSize = 0;
            List<T> closure = new List<T>();
            closure.Add(node);
            while (lastSize != closure.Count)
            {
                lastSize = closure.Count;
                closure.AddRange(DirectMapping(closure));
                closure = closure.Distinct().ToList();
            }
            return closure;
        }

        /// <summary>
        /// Обратное транзитивное замыкание
        /// </summary>
        /// <param name="node">Вершина замыкания</param>
        /// <returns>Набор вершин обратного транзитивного замыкания</returns>
        public List<T> InverseTransitiveClosure(T node)
        {
            int lastSize = 0;
            List<T> closure = new List<T>();
            closure.Add(node);
            while (lastSize != closure.Count)
            {
                lastSize = closure.Count;
                closure.AddRange(InverseMapping(closure));
                closure = closure.Distinct().ToList();
            }
            return closure;
        }

        /// <summary>
        /// Получить матрицу достижимости графа
        /// </summary>
        /// <returns>Матрица достижимости</returns>
        public bool[,] GetAttainbilityMatrix()
        {
            bool[,] mat = new bool[Nodes.Count, Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                List<T> accessibleNodes = DirectTransitiveClosure(Nodes[i]);
                foreach(T node in accessibleNodes)
                {
                    mat[i, Nodes.IndexOf(node)] = true;
                }
            }
            return mat;
        }

        /// <summary>
        /// Получить матрицу контрдостижимости графа
        /// </summary>
        /// <returns>Матрица контрдостижимости</returns>
        public bool[,] GetInaccessibilityMatrix()
        {
            bool[,] mat = new bool[Nodes.Count, Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                List<T> accessibleNodes = InverseTransitiveClosure(Nodes[i]);
                foreach (T node in accessibleNodes)
                {
                    mat[i, Nodes.IndexOf(node)] = true;
                }
            }
            return mat;
        }

        /// <summary>
        /// Разбить граф на сильносвязанные подграфы
        /// </summary>
        /// <returns>Разбитый граф</returns>
        public Graph<Graph<T>> MalgrangePartion()
        {
            //Разбиваем на сильносвязанные подграфы
            List<Graph<T>> subGraphs = new List<Graph<T>>();
            Graph<T> procGraph = this;
            while(procGraph.Nodes.Count != 0)
            {
                List<T> dirClosure = DirectTransitiveClosure(procGraph.Nodes[0]);
                List<T> invClosure = InverseTransitiveClosure(procGraph.Nodes[0]);

                Graph<T> strongSubgraph = new Graph<T>(Intersection(dirClosure, invClosure), this.Edges);
                subGraphs.Add(strongSubgraph);

                procGraph = Difference(procGraph, strongSubgraph);
            }

            //Генерируем связи для конденсаций
            List<Link<Graph<T>>> edges = new List<Link<Graph<T>>>();
            foreach(Graph<T> graphPar in subGraphs)
            {
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
