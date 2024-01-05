USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspAddOrder`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspAddOrder` (IN `UserID` INT)   
BEGIN
	DECLARE id INT;
	DECLARE _uuid VARCHAR(36);
	DECLARE _status VARCHAR(10);
	DECLARE orderExists INT;
	
	SET _status = 'pending';
	SET id = -1;
	SET orderExists = EXISTS(
							SELECT o.*
							FROM tblorder o
							WHERE o.user_id = UserId
							AND o._status = _status);

	IF (orderExists = 0) THEN 	
		SET _uuid = UUID();
		INSERT INTO tblOrder(user_id,_number,created_dtm,_status) VALUE(UserID, _uuid,CURRENT_TIMESTAMP(),_status);
		SET id = LAST_INSERT_ID();

		CALL uspAddAuditMessage(UserId, CONCAT('Add Order (',id,') #',_uuid,')'),'uspAddOrder',0);

	ELSE
		SET id = -2; /* order already exists */
	END IF;
	

 	CALL uspGetOrder(id, UserId, id);  
END//
