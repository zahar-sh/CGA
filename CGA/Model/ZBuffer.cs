namespace CGA.Model
{
    public class ZBuffer : Matrix<float>
    {
        public ZBuffer(int width, int height) : base(width, height)
        {
            Reset();
        }

        public void Reset()
        {
            SetElements((x, y) => float.MaxValue);
        }
    }
}