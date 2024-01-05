USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspIsPhotographyInOrder`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspIsPhotographyInOrder` (IN `OrderId` INT, IN `PhotographyId` BIGINT, IN `UserId` INT)   
BEGIN
	DECLARE itemExists INT;
	
	SET itemExists = EXISTS(
							SELECT oi.id
							FROM tblorder o
							JOIN tblorderitem oi
							ON oi.order_id = o.id
						 	WHERE o.user_id = UserID
						   AND o.id =  OrderId
							AND oi.photography_id = PhotographyId);
	
	IF (itemExists = 1) THEN 	
		SELECT 'true' AS 'Result';
	ELSE
		SELECT 'false' AS 'Result';
	END IF;
END//
