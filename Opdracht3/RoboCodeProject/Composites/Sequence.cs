namespace RoboCodeProject
{
    public class Sequence : BTNode
    {
        private BTNode[] inputNodes;
        public Sequence(BlackBoard blackBoard, params BTNode[] input)
        {
            this.blackBoard = blackBoard;
            inputNodes = input;
        }
        public override BTNodeStatus Tick()
        {
            foreach(BTNode node in inputNodes)
            {
                BTNodeStatus result = node.Tick();

                switch (result)
                {
                    case BTNodeStatus.failed:
                        return BTNodeStatus.failed;
                    case BTNodeStatus.running:
                        return BTNodeStatus.running;
                    case BTNodeStatus.succes:
                        break;
                    default:
                        break;
                }
            }

            return BTNodeStatus.succes;
        }
    }
}
