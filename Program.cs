using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleSocialNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Person> findPathBiBFS(List<Person> people,int source, int destination)
            {
                BFSData sourceData = new BFSData(people.FirstOrDefault(i => i.Id == source));
                BFSData destData = new BFSData(people.FirstOrDefault(i => i.Id == destination));

                while (sourceData.toVisit.Any() && destData.toVisit.Any())
                {
                    Person collision = searchLevel(people, sourceData, destData);
                    if (collision != null)
                    {
                        return mergePaths(sourceData, destData, collision.Id);
                    }

                    collision = searchLevel(people, destData, sourceData);
                    if (collision != null)
                    {
                        return mergePaths(sourceData, destData, collision.Id);
                    }

                }
                return null;
            }

            Person searchLevel(List<Person> people, BFSData primary, BFSData secondary)
            {
                int count = primary.toVisit.Count;
                for(int i = 0; i < count; i++)
                {
                    PathNode pathNode = primary.toVisit.Dequeue();
                    int personId = pathNode.person.Id;

                    if (secondary.visited.ContainsKey(personId))
                    {
                        return pathNode.person;
                    }

                    Person person = pathNode.person;
                    List<int> friends = person.Friends;
                    foreach (int friendId in friends)
                    {
                        if (!primary.visited.ContainsKey(friendId))
                        {
                            Person friend = people.FirstOrDefault(i => i.Id == friendId);
                            PathNode next = new PathNode(friend, pathNode);
                            primary.visited.Add(friendId, next);
                            primary.toVisit.Enqueue(next);

                        }
                    }
                }
                return null;
            }
            
            List<Person> mergePaths(BFSData bfs1,BFSData bfs2,int connection)
            {
                PathNode end1 = bfs1.visited[connection];
                PathNode end2 = bfs2.visited[connection];

                List<Person> pathOne = end1.collapse(false);
                List<Person> pathTwo = end2.collapse(true);

                pathTwo.Remove(pathTwo.First());//remove connection
                pathOne.AddRange(pathTwo);
                return pathOne;
            }
        }
    }

    class BFSData
    {
        public Queue<PathNode> toVisit = new Queue<PathNode>();
        public Dictionary<int, PathNode> visited = new Dictionary<int, PathNode>();

        public BFSData(Person root)
        {
            PathNode sourcePath = new PathNode(root, null);
            toVisit.Enqueue(sourcePath);
            visited.Add(root.Id, sourcePath);
        }

    }

    class PathNode
    {
        public Person person { get; set; }
        private PathNode previousNode = null;
        public PathNode(Person p, PathNode previous)
        {
            person = p;
            previousNode = previous;
        }

        public List<Person> collapse(bool startsWithRoot)
        {
            List<Person> path = new List<Person>();
            PathNode node = this;
            while (node != null)
            {
                if (startsWithRoot)
                {
                    path.Add(node.person);//add to last
                }
                else
                {
                    path.Insert(0,node.person);//add to first
                }
                node = node.previousNode;
            }
            return path;
        }

    }

    class Person
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<int> Friends { get; set; }
    }
}
