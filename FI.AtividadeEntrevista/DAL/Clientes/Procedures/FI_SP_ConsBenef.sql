CREATE PROC FI_SP_ConsBenef
	@IDCLIENTE BIGINT
AS
BEGIN
	IF(ISNULL(@IDCLIENTE,0) = 0)
		SELECT ID, NOME, CPF, IDCLIENTE FROM BENEFICIARIOS WITH(NOLOCK)
	ELSE
		SELECT ID, NOME, CPF, IDCLIENTE FROM BENEFICIARIOS WITH(NOLOCK) WHERE IDCLIENTE = @IDCLIENTE
END