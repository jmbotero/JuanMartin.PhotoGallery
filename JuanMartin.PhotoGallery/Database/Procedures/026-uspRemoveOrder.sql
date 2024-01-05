USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspRemoveOrder`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspRemoveOrder` (IN `OrderId` INT, IN `UserId` INT)   
BEGIN
	DECLARE id INT;
	
	SELECT o.id
	INTO id
	FROM tblorder o
	WHERE o.user_id = UserId
	AND o.id = OrderId;

	IF FOUND_ROWS() THEN 	
		DELETE o.*
		FROM tblorder o
		WHERE o.user_id = UserId
		AND o.id = OrderId;
		
		DELETE oi.*
		FROM tblorderitem oi
		WHERE oi.order_id = OrderId;
	ELSE
		SET id = -1;
	END IF;

	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserId, CONCAT('Remove Order (',OrderId,')'),'uspRemoveOrder',0);
	END IF;
	
	SELECT id;
END//
