module RingMod  (RingMod(), ringMod, rmRem, rmMod) where

--Определите класс типов Группа, https://ru.wikipedia.org/wiki/Группа_(математика).
class Monoid a => Group a where
    ginv   :: a -> a        -- Обратный элемент

--Определите класс типов Кольцо (с единицей), https://ru.wikipedia.org/wiki/Кольцо_(математика).
class Group a => Ring a where
    rmul   :: a -> a -> a   -- Умножение
    runit  :: a             -- Единица

--Определите тип данных Кольцо вычетов по модулю n представителем класса типов Кольцо (с единицей), https://ru.wikipedia.org/wiki/Сравнение_по_модулю#Классы_вычетов.
data RingMod = RingMod { rmRem :: Int, rmMod :: Int } deriving (Show, Eq)

ringMod :: Int -> Int -> RingMod
ringMod a m = RingMod (a `mod` m)  m

instance Monoid RingMod where
    mempty = ringMod 0 1
    mappend (RingMod a m) (RingMod b n) | m == n = ringMod (a + b) m
                                                | otherwise = undefined

instance Group RingMod where
    ginv (RingMod a m) = ringMod (a - m) m

instance Ring RingMod where
    rmul (RingMod a m) (RingMod b n) | m == n = ringMod (a * b) m
                                             | otherwise = undefined
    runit = ringMod 1 1

