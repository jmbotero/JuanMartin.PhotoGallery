USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspAddPhotographyToOrder`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspAddPhotographyToOrder` (IN `PhotographyId` BIGINT, IN `OrderId` INT, IN `UserId` INT)   
BEGIN
	DECLARE id INT;
	DECLARE pos INT;
	
	SET id=-1;

	IF(UserId<>-1) THEN
		SELECT p.id
		INTO id
		FROM tblphotography p
	 	WHERE p.id = PhotographyId;
	 	
		IF FOUND_ROWS() THEN 	
	 	
	 		SELECT COUNT(*)
	 		INTO pos
	 		FROM tblorderitem o
		 	WHERE o.order_id = OrderId;
	 		SET pos = pos + 1;
	 		
			SELECT o.id
			FROM tblorder o
		 	WHERE o.id = OrderId;
			
			IF FOUND_ROWS() THEN
				INSERT INTO tblOrderItem(order_id, photography_id, _index, add_dtm) VALUES(OrderId, PhotographyId, pos, CURRENT_TIMESTAMP());
				SET id=LAST_INSERT_ID();
			ELSE
				SET id = -1;
			END IF;
		ELSE
			SET id = -1;
		END IF;
	END IF;	
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserId, CONCAT('Add photography (',PhotographyId,') to Order (',OrderId,')'),'uspAddPhotographyToOrder',0);
	END IF;

	SELECT id;
END//
