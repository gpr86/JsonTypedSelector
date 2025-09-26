namespace JsonTypedSelector
{
    public class OneOf<L, R>
    {
        private readonly L? _left;
        private readonly R? _right;
        private readonly bool _isLeft;

        public bool IsLeft => _isLeft;
        public bool IsRight => !_isLeft;

        public L Left => IsLeft ? _left! : throw new InvalidOperationException("Result is not Left");
        public R Right => IsRight ? _right! : throw new InvalidOperationException("Result is not Right");

        private OneOf(bool IsLeft, L? Left, R? Right) => (_isLeft, _left, _right) = (IsLeft, Left, Right);

        public static OneOf<L, R> CreateLeft(L value) => new(true, value, default);
        public static OneOf<L, R> CreateRight(R value) => new(false, default, value);
        public static OneOf<L, R> Create(L value) => CreateLeft(value);
        public static OneOf<L, R> Create(R value) => CreateRight(value);

        public static implicit operator OneOf<L, R>(L value) => Create(value);
        public static implicit operator OneOf<L, R>(R value) => Create(value);
        public static implicit operator L(OneOf<L, R> value) => value.Left;
        public static implicit operator R(OneOf<L, R> value) => value.Right;
    }
}
