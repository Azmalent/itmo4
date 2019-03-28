module Utils where
import Datatypes

-- | Операторы сложения и вычитания для времени
(+:) :: Time -> Time -> Time
(Time a) +: (Time b) = Time (a + b) 

(-:) :: Time -> Time -> Time
(Time a) -: (Time b) = Time (a - b) 

-- | Композиция для функции двух аргументов
(.@.) :: (d -> c) -> (a -> b -> d) -> (a -> b -> c)
(f .@. g) x y = f $ g x y

-- | Находит все подмножества списка
subsets :: [a] -> [[a]]
subsets []  = [[]]
subsets (x:xs) = subsets xs ++ map (x:) (subsets xs)

-- | Возвращает длину интервала
duration :: Interval -> Time
duration (Interval (Time a) (Time b)) = Time (b - a)

-- | Проверяет, включает ли один интервал времени другой
includes :: Interval -> Interval -> Bool
includes i (Interval a b) = (i `includesTime` a) && (i `includesTime` b)
    where includesTime (Interval a b) t = t `between` (a, b)

-- | Проверяет попадание в интервал
between :: Ord a => a -> (a, a) -> Bool
a `between` (b, c) = (a >= b) && (a <= c)

-- | Cдвиг дежурства для охранника
shift :: Time -> Guard -> Guard
shift t (Guard id watch) = Guard id $ Interval t (t +: duration watch)

shiftEnd :: Time -> Guard -> Guard
shiftEnd t (Guard id watch) = Guard id $ Interval (t -: duration watch) t