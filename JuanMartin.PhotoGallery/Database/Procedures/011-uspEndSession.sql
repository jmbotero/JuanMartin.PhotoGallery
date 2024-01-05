USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspEndSession`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspEndSession` (IN `SessionID` INT)   
BEGIN
	DECLARE id INT;
	
	SELECT s.id 
	INTO id
	FROM tblSession s	
 	WHERE s.id = SessionID;
	
	IF FOUND_ROWS() THEN 	
		UPDATE tblSession s
		SET s.end_dtm = CURRENT_TIMESTAMP()
	 	WHERE s.id = SessionID;
	ELSE
		SET id=-1;
	END IF;
	
	SELECT id;
END//
