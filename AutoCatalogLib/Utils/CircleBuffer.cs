namespace AutoCatalogLib.Utils
{
    public class CircleBuffer<TElement>
    {
        public TElement Element { get; set; }
        public CircleBuffer<TElement> Next { get; private set; }
        public bool Empty { get; set; }

        private CircleBuffer()
        {
            Empty = true;
        }

        public static CircleBuffer<TElement> BuildBuffer(int size)
        {
            var head = new CircleBuffer<TElement>();
            var hop = head;

            for (int iteration = 1; iteration < size; iteration++)
            {
                hop.Next = new CircleBuffer<TElement>();
                hop = hop.Next;
            }

            hop.Next = head;

            return head;
        }
    }
}
