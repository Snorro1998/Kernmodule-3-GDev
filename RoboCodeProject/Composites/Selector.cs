using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboCodeProject
{

    /// <summary>
    /// Selector runs input nodes and returns succes if one input node returns success
    /// </summary>
    public class Selector : BTNode
    {
        private BTNode[] inputNodes;
        public Selector(BlackBoard blackBoard, params BTNode[] input)
        {
            this.blackBoard = blackBoard;
            inputNodes = input;
        }
        public override BTNodeStatus Tick()
        {
            foreach (BTNode node in inputNodes)
            {
                BTNodeStatus result = node.Tick();

                switch (result)
                {
                    case BTNodeStatus.failed:
                        continue;
                    case BTNodeStatus.running:
                        return BTNodeStatus.running;
                    case BTNodeStatus.succes:
                        return BTNodeStatus.succes;
                }
            }

            return BTNodeStatus.failed;
        }
    }
}
