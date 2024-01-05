USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspAddPhotography`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspAddPhotography` (IN `ImageSource` INT, IN `FileName` VARCHAR(50), IN `FilePath` VARCHAR(255), IN `Title` VARCHAR(100))   
BEGIN
	DECLARE id BIGINT;
	DECLARE response INT;
	
	SET response = -1;

	SELECT pic.id 
	INTO id
	FROM tblphotography pic
 	WHERE pic.filename = FileName
	AND pic._path = FilePath;
	
	IF NOT FOUND_ROWS() THEN 	
		SET response = 1;
		INSERT INTO tblPhotography (_source,filename, _path, title) VALUE(ImageSource,FileName, FilePath, Title);
		SET id=LAST_INSERT_ID();
	ELSE
		SET response = -2;
	END IF;	

	SELECT response, id;
END//
