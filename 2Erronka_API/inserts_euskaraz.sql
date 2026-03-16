USE `2mg3_2erronka`;

-- Kanpo gakoen egiaztapena desgaitu datuak sartu bitartean, ordena arazoak ekiditeko
SET FOREIGN_KEY_CHECKS = 0;

-- Taulak garbitu (aukerakoa, baina gomendagarria datu garbiak izateko)
TRUNCATE TABLE `eskariak_has_produktuak`;
TRUNCATE TABLE `produktuak_has_osagaiak`;
TRUNCATE TABLE `eskariak`;
TRUNCATE TABLE `erreserbak`;
TRUNCATE TABLE `osagaiak`;
TRUNCATE TABLE `produktuak`;
TRUNCATE TABLE `mota`;
TRUNCATE TABLE `mahaiak`;
TRUNCATE TABLE `langileak`;
TRUNCATE TABLE `hornitzaileak`;
TRUNCATE TABLE `lanpostuak`;

-- 1. Lanpostuak
INSERT INTO `lanpostuak` (`id`, `lanpostua`) VALUES 
(1, 'Gerentea'),
(2, 'Zerbitzaria'),
(3, 'Sukaldaria'),
(4, 'Administratzailea');

-- 2. Hornitzaileak
INSERT INTO `hornitzaileak` (`id`, `izena`, `kontaktua`, `helbidea`) VALUES 
(1, 'Eroski Banaketa', '944123456', 'Elorrio Kalea 10, Bizkaia'),
(2, 'Makro Elikagaiak', '943987654', 'Oiartzun Industrialdea, Gipuzkoa'),
(3, 'Harategi Berria', '945555666', 'Gasteizko Plaza, Araba');

-- 3. Mota (Produktuentzat)
INSERT INTO `mota` (`id`, `izena`) VALUES 
(1, 'Edaria'),
(2, 'Janaria'),
(3, 'Postrea');

-- 4. Mahaiak
INSERT INTO `mahaiak` (`id`, `zenbakia`, `pertsona_kopuru`, `kokapena`) VALUES 
(1, 1, 4, 'Egongela Nagusia'),
(2, 2, 2, 'Terraza'),
(3, 3, 6, 'Leiho ondoan'),
(4, 4, 4, 'Sukalde ondoan');

-- 5. Langileak (Pasahitza: "1234" SHA256 hasha)
INSERT INTO `langileak` (`id`, `izena`, `abizena`, `NAN`, `erabiltzaile_izena`, `langile_kodea`, `pasahitza`, `helbidea`, `lanpostuak_id`) VALUES 
(1, 'Ane', 'Arrieta', '12345678A', 'ane1', 101, '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Kale Nagusia 1, Bilbo', 1),
(2, 'Jon', 'Mendizabal', '87654321B', 'jon2', 102, '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Araba Kalea 5, Donostia', 2),
(3, 'Miren', 'Lasa', '11223344C', 'miren3', 103, '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Gasteiz Hiribidea 12, Gasteiz', 3);

-- 6. Produktuak
INSERT INTO `produktuak` (`id`, `izena`, `prezioa`, `stock`, `mota_id`) VALUES 
(1, 'Coca-Cola', 2.50, 50, 1),
(2, 'Kaiku Esnea', 1.20, 100, 1),
(3, 'Tortilla Pintxoa', 1.80, 20, 2),
(4, 'Bakailaoa Bizkaitar Erara', 15.50, 15, 2),
(5, 'Gazta Tarta', 4.50, 10, 3);

-- 7. Osagaiak
INSERT INTO `osagaiak` (`id`, `izena`, `prezioa`, `stock`, `hornitzaileak_id`) VALUES 
(1, 'Patata', 0.50, 200, 1),
(2, 'Arrautza', 0.20, 150, 2),
(3, 'Bakailao Freskoa', 8.00, 30, 3),
(4, 'Gazta Krematsua', 3.50, 20, 1),
(5, 'Coca-Cola botila', 1.00, 100, 1); -- Produktu bezala saltzen dena, baina osagai/stock bezala kudeatua

-- 8. Erreserbak
INSERT INTO `erreserbak` (`id`, `bezero_izena`, `telefonoa`, `pertsona_kopurua`, `eguna_ordua`, `prezio_totala`, `faktura_ruta`, `langileak_id`, `mahaiak_id`) VALUES 
(1, 'Gorka Urkizu', '600111222', 4, '2026-03-20 14:30:00', 45.50, 'tiketak/tiket_1.pdf', 1, 1),
(2, 'Idurre Lopez', '655444333', 2, '2026-03-21 21:00:00', 25.00, NULL, 2, 2);

-- 9. Eskariak
INSERT INTO `eskariak` (`id`, `prezioa`, `egoera`, `erreserbak_id`, `erreserbak_langileak_id`, `erreserbak_mahaiak_id`) VALUES 
(1, 18.50, 'Bukatua', 1, 1, 1),
(2, 12.00, 'Prestatzen', 2, 2, 2);

-- 10. Produktuak eta Osagaiak harremana
INSERT INTO `produktuak_has_osagaiak` (`produktuak_id`, `osagaiak_id`, `kantitatea`) VALUES 
(1, 5, 1), -- Coca-Cola (produktua) -> Coca-Cola botila (osagaia) (1 unitate)
(3, 1, 3), -- Tortilla -> Patata (3 unitate)
(3, 2, 2), -- Tortilla -> Arrautza (2 unitate)
(4, 3, 1); -- Bakailaoa -> Bakailao Freskoa (1 unitate)

-- 11. Eskariak eta Produktuak harremana
INSERT INTO `eskariak_has_produktuak` (`eskariak_id`, `produktuak_id`, `kantitatea`, `prezioa`) VALUES 
(1, 1, 2, 5.00),
(1, 3, 2, 3.60),
(2, 4, 1, 15.50);

-- Egiaztapena berriro gaitu
SET FOREIGN_KEY_CHECKS = 1;
