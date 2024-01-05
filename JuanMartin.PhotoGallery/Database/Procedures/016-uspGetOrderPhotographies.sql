USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspGetOrderPhotographies`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetOrderPhotographies` (IN `UserId` INT, IN `OrderId` INT, IN `CurrentPage` INT, IN `PageSize` INT)   
BEGIN
	DECLARE rec_skip INT;
	DECLARE rec_take INT;
	
	SET rec_take = PageSize;
	SET rec_skip = (CurrentPage - 1) * PageSize;

	SELECT v.*
	FROM (SELECT @p1:=UserId p) parm ,tblorderitem o 
	JOIN vwphotographywithranking v 
		ON v.id = o.photography_id
	WHERE o.order_id = OrderId
	ORDER BY o._index ASC
	LIMIT rec_take
	OFFSET rec_skip;
END//
