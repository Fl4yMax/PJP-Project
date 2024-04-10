//using Grammar;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Lab3
//{
//    public class GrammarOps
//    {
//        private readonly Terminal epsilon = new("{e}");
//        public GrammarOps(IGrammar g)
//        {
//            this.g = g;
//            Compute_empty();
//            ComputeFirst();
//        }

//        public ISet<Nonterminal> EmptyNonterminals { get; } = new HashSet<Nonterminal>();
//        public Dictionary<Rule, HashSet<Terminal>> Firsts { get; } = new();
//        private void Compute_empty()
//        {
//            foreach (var rule in g.Rules)
//            {
//                if (rule.RHS.Count == 0)
//                {
//                    EmptyNonterminals.Add(rule.LHS);
//                }
//            }

//            int previousSize = EmptyNonterminals.Count;
//            do
//            {
//                previousSize = EmptyNonterminals.Count;
//                foreach (var rule in g.Rules)
//                {
//                    if (rule.RHS.All(x => x is Nonterminal nonterminal && EmptyNonterminals.Contains(nonterminal)))
//                    {
//                        EmptyNonterminals.Add(rule.LHS);
//                    }
//                }
//            } while (previousSize != EmptyNonterminals.Count);

//        }
//        private static void PrintTable(Dictionary<Nonterminal, (HashSet<Nonterminal> Left, HashSet<Terminal> Right)> table)
//        {
//            foreach (var (n, (left, right)) in table)
//            {
//                Console.WriteLine($"{n} | {string.Join(" ", left)} | {string.Join(" ", right)}");
//            }
//            Console.WriteLine();
//        }
//        private static Dictionary<Nonterminal, (HashSet<Nonterminal> Left, HashSet<Terminal> Right)> Compute(
//            Dictionary<Nonterminal, (HashSet<Nonterminal> Left, HashSet<Terminal> Right)> table)
//        {
//            bool isChanged;
//            do
//            {
//                isChanged = false;
//                foreach (var n in table.Keys.ToList())
//                {
//                    var (newLeft, newRight) = table[n];
//                    foreach (var nonterminal in table[n].Left)
//                    {
//                        var toAdd = table[nonterminal];
//                        newLeft = toAdd.Left.Union(newLeft).ToHashSet();
//                        newRight = toAdd.Right.Union(newRight).ToHashSet();
//                        table[n] = (newLeft, newRight);
//                    }
//                    if (newLeft.Count != table[n].Left.Count || newRight.Count != table[n].Right.Count) isChanged = true;
//                }
//            } while (isChanged);
//            return table;
//        }
//        private void ComputeFirst()
//        {
//            Dictionary<Nonterminal, (HashSet<Nonterminal> Left, HashSet<Terminal> Right)> unifiedFirsts = new();
//            foreach (var rule in g.Rules)
//            {
//                var left = new HashSet<Nonterminal>();
//                var right = new HashSet<Terminal>();
//                foreach (var symbol in rule.RHS)
//                {
//                    if (symbol is Terminal t) { right.Add(t); break; };
//                    if (symbol is Nonterminal n)
//                    {
//                        left.Add(n);
//                        if (!EmptyNonterminals.Contains(n)) break;
//                    }
//                }

//                if (unifiedFirsts.ContainsKey(rule.LHS))
//                {
//                    var newleft = unifiedFirsts[rule.LHS].Left.Union(left).ToHashSet();
//                    var newRight = unifiedFirsts[rule.LHS].Right.Union(right).ToHashSet();
//                    unifiedFirsts[rule.LHS] = (newleft, newRight);
//                }
//                else
//                {
//                    unifiedFirsts.Add(rule.LHS, (left, right));
//                }
//            }
//            PrintTable(unifiedFirsts);
//            unifiedFirsts = Compute(unifiedFirsts);
//            foreach (var rule in g.Rules)
//            {
//                var first = new HashSet<Terminal>();
//                int position = 0;
//                foreach (var symbol in rule.RHS)
//                {
//                    if (symbol is Terminal t) { first.Add(t); break; };
//                    if (symbol is Nonterminal n)
//                    {
//                        first = first.Union(unifiedFirsts[n].Right).ToHashSet();
//                        if (!EmptyNonterminals.Contains(n)) break;
//                    }
//                    position++;
//                }
//                if (position == rule.RHS.Count) first.Add(epsilon);
//                Firsts[rule] = first;
//            }
//        }

//        private void ComputeFollow()
//        {
//            Dictionary<Nonterminal, (HashSet<Nonterminal> Left, HashSet<Terminal> Right)> unifiedFollows = new();


//        }

//        private IGrammar g;
//    }
//}