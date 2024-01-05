USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspUpdatePhotographyDetails`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspUpdatePhotographyDetails` (IN `UserId` INT, IN `PhotographyId` BIGINT, IN `Location` VARCHAR(50))  NO SQL 
BEGIN
	DECLARE id INT;
	DECLARE imageExists INT;
	DECLARE linkExists INT;
	
	SET imageExists = EXISTS(
	            SELECT p.filename 
					FROM tblphotography p
				 	WHERE p.id = PhotographyId);

	SET id = -1;
					 
	IF (imageExists = 1) THEN 	
		SELECT l.id 
		INTO id
		FROM tblLocation l
	 	WHERE l._reference = Location;
		
		IF NOT FOUND_ROWS() THEN 	
			INSERT INTO tblLocation(_reference) VALUE(Location);
			SET id=LAST_INSERT_ID();
		END IF;
		
		IF(PhotographyId<>-1 AND id > 0) THEN
			UPDATE tblphotography p
				SET p.location_id = id
			WHERE p.id = PhotographyId;
		END IF;
	END IF;
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Update location ''', Location , ''' (', id , ') for photography (', PhotographyId ,')'),'uspUpdatePhotographyDetails',0);
	END IF;
	SELECT id;
END//
