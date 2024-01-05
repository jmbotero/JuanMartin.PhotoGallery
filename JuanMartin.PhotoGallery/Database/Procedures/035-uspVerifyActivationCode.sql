USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspVerifyActivationCode`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspVerifyActivationCode` (IN `ActivationCode` VARCHAR(36))   
BEGIN
	DECLARE VerifyDtm DATETIME;
	DECLARE RequestDtm DATETIME;
	DECLARE errorCode INT;
	DECLARE login VARCHAR(50);
	DECLARE userId INT;
	
	SET verifyDtm = CURRENT_TIMESTAMP();
	SET login = '';
	SET userId = -1;
	
	SELECT r.request_dtm, r.user_id
	INTO requestDtm,userId
	FROM tblpasswordreset r
	WHERE  r.activation_code = ActivationCode;

	SET errorCode = -1; /* code not found */
	IF FOUND_ROWS() THEN 	
		SET errorCode = 1; /* code good */
		SELECT u.login
		INTO login
		FROM tblUser u
	 	WHERE u.id = userId;

		IF TIMESTAMPDIFF(MINUTE,requestDtm, verifyDtm) > 10 THEN
			DELETE r.*
			FROM tblpasswordreset r
			WHERE  r.activation_code = ActivationCode;
	
			SET errorCode = -2; /* code expired */
		END IF;
	END IF;
	
	SELECT userId AS 'Id', login AS 'Login', errorCode AS 'ErrorCode';
END//
