namespace ASK.Filters.Operations;

public interface IOperation;

public abstract record UnaryOperation(IOperation Operation) : IOperation;
public abstract record BinaryOperation(IOperation LeftOperation, IOperation RightOperation) : IOperation;

public record AndOperation(IOperation LeftOperation, IOperation RightOperation) : BinaryOperation(LeftOperation, RightOperation);

public record OrOperation(IOperation LeftOperation, IOperation RightOperation) : BinaryOperation(LeftOperation, RightOperation);

public record NotOperation(IOperation Operation) : UnaryOperation(Operation);