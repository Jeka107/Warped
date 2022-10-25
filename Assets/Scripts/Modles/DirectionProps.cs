public enum SIGN { POSITIVE, NEGATIVE};
public enum AXIS { X, Y, Z };

public class DirectionProps
{
    public SIGN _sign;
    public AXIS _axis;

    public DirectionProps(SIGN sign, AXIS axis)
    {
        _sign = sign;
        _axis = axis;
    }
}
