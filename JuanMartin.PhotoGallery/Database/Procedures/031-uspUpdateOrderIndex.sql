USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspUpdateOrderIndex`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspUpdateOrderIndex` (IN `UserId` INT, IN `OrderId` INT, IN `PhotographyId` BIGINT, IN `_Index` INT)   
BEGIN
	DECLARE previousIndex INT;
	
	SELECT oi._index
	INTO previousIndex
	FROM tblorderitem oi
	WHERE oi.order_id = OrderId
	AND oi.photography_id = PhotographyId;

	IF(previousIndex <> _Index) THEN
		UPDATE tblorderitem oi
			SET oi._index = _Index,
				 oi.update_dtm = CURRENT_TIMESTAMP()
		WHERE oi.order_id = OrderId
		AND oi.photography_id = PhotographyId;
			
		CALL uspAddAuditMessage(UserID, CONCAT('Changed index of Photography (',PhotographyId,') in order  (',OrderId,') to: ',_Index,'.'),'uspUpdateOrderIndex',0);
	END IF;
END//
