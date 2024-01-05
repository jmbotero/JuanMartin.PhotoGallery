USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspRemovePhotographyFromOrder`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspRemovePhotographyFromOrder` (IN `PhotographyId` BIGINT, IN `OrderId` INT, IN `UserId` INT)   
BEGIN
	DECLARE id INT;
	
	SELECT oi.id
	INTO id
	FROM tblorder o
	JOIN tblorderitem oi
	ON oi.order_id = o.id
 	WHERE o.user_id = UserID
   AND o.id =  OrderId
	AND oi.photography_id = PhotographyId;
	
	IF FOUND_ROWS() THEN 	
		DELETE oi.*
		FROM tblorder o
		JOIN tblorderitem oi
		ON oi.order_id = o.id
	 	WHERE o.user_id = UserID
	   AND o.id =  OrderId
		AND oi.photography_id = PhotographyId;
	ELSE
		SET id=-1;
	END IF;
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserId, CONCAT('Remove photography (',PhotographyId,') from Order (',OrderId,')'),'uspRemovePhotographyFromOrder',0);
	END IF;

	SELECT id;
END//
