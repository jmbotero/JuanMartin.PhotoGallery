USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspGetOrder`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetOrder` (IN `OrderId` INT, IN `UserId` INT, IN `ErrorId` INT)  COMMENT 'The ErrorId should have a default value but always pass -1 if you don''t want this ErrporId to overwrite the otder id in the response.' 
BEGIN
	DECLARE orderExists INT;
	DECLARE cnt INT;

	IF (ErrorId=-1) THEN 		
		SET orderExists = EXISTS(
								SELECT o.id
								FROM tblorder o
								WHERE o.id = OrderId
								AND o.user_id = UserId);
	
		IF (orderExists = 0) THEN 	
			SELECT -1 AS 'Id',
					 '' AS 'Number',
					 UserId AS 'UserId',
					 NOW() AS 'CreatedDtm',
					 0 AS 'Count',
					 '' AS 'Status';
		ELSE
	 		SELECT COUNT(*)
	 		INTO cnt
	 		FROM tblorderitem o
		 	WHERE o.order_id = OrderId;
		 	
			SELECT o.id AS 'Id',
				 o._number AS 'Number',
				 o.user_id  AS 'UserId',
				 o.created_dtm AS 'CreatedDtm',
				 cnt AS 'Count',
				 o._status AS 'Status'
			FROM tblorder o
			WHERE o.id = OrderId
			AND o.user_id = UserId;
		END IF;
	ELSE
		SELECT ErrorId AS 'Id',
				 '' AS 'Number',
				 UserId AS 'UserId',
				 NOW() AS 'CreatedDtm',
					 0 AS 'Count',
				 '' AS 'Status';
	END IF;
END//
