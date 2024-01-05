USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspGetCurrentActiveOrder`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetCurrentActiveOrder` (IN `UserId` INT)  NO SQL 
BEGIN
 	DECLARE oid INT;
 	
 	SELECT MAX(o.id)
 	INTO oid
 	FROM tblorder o
 	WHERE o.user_id = UserId
 	AND o._status = 'pending'
	ORDER BY o.created_dtm DESC;
	 	
 	IF (oid = NULL) THEN 	
		SET oid = -1;
	END IF;

 	CALL uspGetOrder(oid, UserId, -1);
END//
