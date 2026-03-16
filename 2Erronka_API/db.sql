-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema 2mg3_2erronka
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema 2mg3_2erronka
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `2mg3_2erronka` DEFAULT CHARACTER SET utf8 ;
USE `2mg3_2erronka` ;

-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`lanpostuak`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`lanpostuak` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `lanpostua` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`langileak`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`langileak` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `izena` VARCHAR(20) NOT NULL,
  `abizena` VARCHAR(45) NOT NULL,
  `NAN` VARCHAR(9) NOT NULL,
  `erabiltzaile_izena` VARCHAR(20) NOT NULL,
  `langile_kodea` INT NOT NULL,
  `pasahitza` LONGTEXT NOT NULL,
  `helbidea` VARCHAR(100) NOT NULL,
  `lanpostuak_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_langileak_lanpostuak1_idx` (`lanpostuak_id` ASC) VISIBLE,
  CONSTRAINT `fk_langileak_lanpostuak1`
    FOREIGN KEY (`lanpostuak_id`)
    REFERENCES `2mg3_2erronka`.`lanpostuak` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`mahaiak`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`mahaiak` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `zenbakia` INT NOT NULL,
  `pertsona_kopuru` INT NOT NULL,
  `kokapena` VARCHAR(25) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`erreserbak`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`erreserbak` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `bezero_izena` VARCHAR(45) NOT NULL,
  `telefonoa` VARCHAR(9) NOT NULL,
  `pertsona_kopurua` INT NOT NULL,
  `eguna_ordua` DATETIME NOT NULL,
  `prezio_totala` DOUBLE NOT NULL,
  `faktura_ruta` VARCHAR(45) NULL,
  `langileak_id` INT NOT NULL,
  `mahaiak_id` INT NOT NULL,
  PRIMARY KEY (`id`, `langileak_id`, `mahaiak_id`),
  INDEX `fk_erreserbak_langileak1_idx` (`langileak_id` ASC) VISIBLE,
  INDEX `fk_erreserbak_mahaiak1_idx` (`mahaiak_id` ASC) VISIBLE,
  CONSTRAINT `fk_erreserbak_langileak1`
    FOREIGN KEY (`langileak_id`)
    REFERENCES `2mg3_2erronka`.`langileak` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_erreserbak_mahaiak1`
    FOREIGN KEY (`mahaiak_id`)
    REFERENCES `2mg3_2erronka`.`mahaiak` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`eskariak`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`eskariak` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `prezioa` DOUBLE NOT NULL,
  `egoera` VARCHAR(45) NOT NULL,
  `erreserbak_id` INT NOT NULL,
  `erreserbak_langileak_id` INT NOT NULL,
  `erreserbak_mahaiak_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_eskariak_erreserbak1_idx` (`erreserbak_id` ASC, `erreserbak_langileak_id` ASC, `erreserbak_mahaiak_id` ASC) VISIBLE,
  CONSTRAINT `fk_eskariak_erreserbak1`
    FOREIGN KEY (`erreserbak_id` , `erreserbak_langileak_id` , `erreserbak_mahaiak_id`)
    REFERENCES `2mg3_2erronka`.`erreserbak` (`id` , `langileak_id` , `mahaiak_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`mota`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`mota` (
  `id` INT NOT NULL,
  `izena` VARCHAR(45) NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`produktuak`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`produktuak` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `izena` VARCHAR(60) NOT NULL,
  `prezioa` DOUBLE NOT NULL,
  `stock` INT NOT NULL,
  `mota_id` INT NOT NULL,
  PRIMARY KEY (`id`, `mota_id`),
  INDEX `fk_produktuak_mota1_idx` (`mota_id` ASC) VISIBLE,
  CONSTRAINT `fk_produktuak_mota1`
    FOREIGN KEY (`mota_id`)
    REFERENCES `2mg3_2erronka`.`mota` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`hornitzaileak`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`hornitzaileak` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `izena` VARCHAR(45) NOT NULL,
  `kontaktua` VARCHAR(50) NOT NULL,
  `helbidea` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`osagaiak`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`osagaiak` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `izena` VARCHAR(20) NOT NULL,
  `prezioa` DOUBLE NOT NULL,
  `stock` INT NOT NULL,
  `hornitzaileak_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_osagaiak_hornitzaileak1_idx` (`hornitzaileak_id` ASC) VISIBLE,
  CONSTRAINT `fk_osagaiak_hornitzaileak1`
    FOREIGN KEY (`hornitzaileak_id`)
    REFERENCES `2mg3_2erronka`.`hornitzaileak` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`materialak`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`materialak` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `izena` VARCHAR(20) NOT NULL,
  `prezioa` DOUBLE NOT NULL,
  `stock` INT NOT NULL,
  `hornitzaileak_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_materialak_hornitzaileak1_idx` (`hornitzaileak_id` ASC) VISIBLE,
  CONSTRAINT `fk_materialak_hornitzaileak1`
    FOREIGN KEY (`hornitzaileak_id`)
    REFERENCES `2mg3_2erronka`.`hornitzaileak` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`produktuak_has_osagaiak`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`produktuak_has_osagaiak` (
  `produktuak_id` INT NOT NULL,
  `osagaiak_id` INT NOT NULL,
  `kantitatea` INT NOT NULL,
  PRIMARY KEY (`produktuak_id`, `osagaiak_id`),
  INDEX `fk_produktuak_has_osagaiak_osagaiak1_idx` (`osagaiak_id` ASC) VISIBLE,
  INDEX `fk_produktuak_has_osagaiak_produktuak1_idx` (`produktuak_id` ASC) VISIBLE,
  CONSTRAINT `fk_produktuak_has_osagaiak_produktuak1`
    FOREIGN KEY (`produktuak_id`)
    REFERENCES `2mg3_2erronka`.`produktuak` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_produktuak_has_osagaiak_osagaiak1`
    FOREIGN KEY (`osagaiak_id`)
    REFERENCES `2mg3_2erronka`.`osagaiak` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`eskariak_has_produktuak`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`eskariak_has_produktuak` (
  `eskariak_id` INT NOT NULL,
  `produktuak_id` INT NOT NULL,
  `kantitatea` INT NOT NULL,
  `prezioa` DOUBLE NOT NULL,
  PRIMARY KEY (`eskariak_id`, `produktuak_id`),
  INDEX `fk_eskariak_has_produktuak_produktuak1_idx` (`produktuak_id` ASC) VISIBLE,
  INDEX `fk_eskariak_has_produktuak_eskariak1_idx` (`eskariak_id` ASC) VISIBLE,
  CONSTRAINT `fk_eskariak_has_produktuak_eskariak1`
    FOREIGN KEY (`eskariak_id`)
    REFERENCES `2mg3_2erronka`.`eskariak` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_eskariak_has_produktuak_produktuak1`
    FOREIGN KEY (`produktuak_id`)
    REFERENCES `2mg3_2erronka`.`produktuak` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `2mg3_2erronka`.`produktuak_has_osagaiak1`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `2mg3_2erronka`.`produktuak_has_osagaiak1` (
  `produktuak_id` INT NOT NULL,
  `osagaiak_id` INT NOT NULL,
  `kantitatea` INT NOT NULL,
  PRIMARY KEY (`produktuak_id`, `osagaiak_id`),
  INDEX `fk_produktuak_has_osagaiak1_osagaiak1_idx` (`osagaiak_id` ASC) VISIBLE,
  INDEX `fk_produktuak_has_osagaiak1_produktuak1_idx` (`produktuak_id` ASC) VISIBLE,
  CONSTRAINT `fk_produktuak_has_osagaiak1_produktuak1`
    FOREIGN KEY (`produktuak_id`)
    REFERENCES `2mg3_2erronka`.`produktuak` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_produktuak_has_osagaiak1_osagaiak1`
    FOREIGN KEY (`osagaiak_id`)
    REFERENCES `2mg3_2erronka`.`osagaiak` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
