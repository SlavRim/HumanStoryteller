﻿using System;
using System.Collections;
using System.Collections.Generic;
using HumanStoryteller.CheckConditions;
using HumanStoryteller.Model;
using RimWorld;
using Verse;

namespace HumanStoryteller {
    public class StoryGraph : IExposable {
        private StoryNode _root;
        private int _lastTransition;

        public StoryGraph() {
        }

        public StoryGraph(StoryNode root) {
            _root = root;
        }

        public StoryNode TryNewEvent(StoryEventNode current, int bigTick) {
            int tickPassed = bigTick - _lastTransition;
            var storyNode = current?.StoryNode;
            if (current?.StoryNode == null
                || storyNode.LeftChild == null && storyNode.RightChild == null
                || storyNode.LeftChild != null && storyNode.LeftChild.Offset > tickPassed
                                             && storyNode.RightChild != null && storyNode.RightChild.Offset > tickPassed) {
                return null;
            }

            Connection next;
            List<CheckCondition> conditions = storyNode.Conditions;

            if (conditions != null) {
                bool allTrue = true;
                for (var i = 0; i < conditions.Count; i++) {
                    var condition = conditions[i];
                    if (condition.Check(current.Result, i)) continue;
                    allTrue = false;
                    break;
                }

                next = allTrue ? storyNode.LeftChild : storyNode.RightChild;
            } else {
                next = storyNode.LeftChild;
            }

            if (next == null) {
                return null;
            }

            if (next.Offset <= tickPassed) {
                _lastTransition = bigTick;
                return next.Node;
            }

            return null;
        }

        public StoryNode GetCurrentNode(String uuid) {
            if (uuid == null) return null;
            return findNodeById(_root, uuid, new List<string>());
        }

        private StoryNode findNodeById(StoryNode current, String uuid, List<string> processed) {
            if (current == null) {
                return null;
            }

            string currentUuid = current.StoryEvent.Uuid;
            
            if (currentUuid.Equals(uuid)) {
                return current;
            }

            if (processed.Contains(currentUuid)) {
                return null;//Avoid infinite loops
            }

            processed.Add(currentUuid);

            StoryNode leftAnswer = findNodeById(current.LeftChild?.Node, uuid, processed);
            if (leftAnswer != null) {
                return leftAnswer;
            }

            return findNodeById(current.RightChild?.Node, uuid, processed);
        }

        public List<StoryNode> GetAllNodes() {
            return GetAllNodes(_root, new List<StoryNode>());
        }

        private List<StoryNode> GetAllNodes(StoryNode current, List<StoryNode> storyNodes) {
            if (current == null) {
                return storyNodes;
            }
            
            if (storyNodes.Contains(current)) {
                return storyNodes;
            }

            storyNodes.Add(current);

            GetAllNodes(current.LeftChild?.Node, storyNodes);
            return GetAllNodes(current.RightChild?.Node, storyNodes);
        }

        public StoryNode Root => _root;

        public override string ToString() {
            return $"Root: {_root}, LastTransition: {_lastTransition}";
        }
        
        public void ExposeData() {
            Scribe_References.Look(ref _root, "storyNode");
            Scribe_Values.Look(ref _lastTransition, "lastTransaction");
        }
    }
}