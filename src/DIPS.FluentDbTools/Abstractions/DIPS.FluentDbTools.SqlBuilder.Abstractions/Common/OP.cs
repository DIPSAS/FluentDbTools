namespace DIPS.FluentDbTools.SqlBuilder.Abstractions.Common
{
    public enum OP
    {
        /// <summary>
        /// Equal
        /// </summary>
        EQ,
        /// <summary>
        /// Less than
        /// </summary>
        LT,
        /// <summary>
        /// Greater Than
        /// </summary>
        GT,
        /// <summary>
        /// Different
        /// </summary>
        DI,
        NULL_OR_DI,
        /// <summary>
        /// Is
        /// </summary>
        IS,
        /// <summary>
        /// Is Not
        /// </summary>
        ISNOT,

        /// <summary>
        /// Less than or equal <=
        /// </summary>
        LTEQ,

        /// <summary>
        /// Greater than or equal >=
        /// </summary>
        GTEQ,

        /// <summary>
        /// IN (value)
        /// </summary>
        IN,

        /// <summary>
        /// NOT IN (value)
        /// </summary>
        NOT_IN
    }
}