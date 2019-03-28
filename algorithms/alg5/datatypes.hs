module Datatypes where

-- | Момент времени
newtype Time = Time 
    { minutes :: Int 
    } deriving (Eq, Ord)

instance Show Time where
    show t = show' (m `div` 60) ++ ":" ++ show' (m `mod` 60)
             where m = minutes t
                   show' t | t < 0 && t > -10 = "-0" ++ show (-t)
                           | t >= 0 && t < 10  = "0" ++ show t 
                           | otherwise = show t

-- | Временной интервал
data Interval = Interval 
    { start :: Time
    , end   :: Time 
    } deriving (Eq)

instance Show Interval where
    show (Interval a b) = (show a ++ " - " ++ show b)

-- | Охранники
data Guard = Guard 
    { id'   :: Int
    , watch :: Interval 
    } deriving (Eq, Show)

type Schedule = [Guard]

-- | Входные данные
data InputData = InputData 
    { endTime     :: Time
    , schedule    :: Schedule
    , watchLength :: Time 
    } deriving Show