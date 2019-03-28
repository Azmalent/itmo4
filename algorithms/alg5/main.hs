import Data.List (nub, sort, find, (\\))
import Control.Exception (assert)
import Datatypes
import Input
import Test
import Utils

-- | Возвращает интервалы времени из расписания охраны.
getIntervals :: Time -> Schedule -> [Interval]
getIntervals end = join . sort . nub . addBorders . concatPairs . map getTime
    where getTime (Guard _ (Interval a b)) = (a, b)
          concatPairs = uncurry (++) . unzip
          addBorders xs = let start = Time 0 in start:end:xs
          join xs = zipWith Interval xs (tail xs)

-- | Для каждого интервала находит количество охранников.
intervalsByGuardCount :: Time -> Schedule -> [(Interval, Int)]
intervalsByGuardCount end [] = error "Не задан график дежурства охранников"
intervalsByGuardCount end gs = map guardCount $ getIntervals end gs
    where guardCount i = let guards = filter (\(Guard _ watch) -> watch `includes` i) gs
                         in (i, length guards)

-- | Находит интервалы времени с недостаточной охраной.
weakGuardIntervals :: Time -> Schedule -> [(Interval, Int)]
weakGuardIntervals = filter (\(_, n) -> n < 2) .@. intervalsByGuardCount

-- | Находит интервалы времени с хорошей охраной.
strongGuardIntervals :: Time -> Schedule -> [(Interval, Int)]
strongGuardIntervals = filter (\(_, n) -> n > 2) .@. intervalsByGuardCount

-- | Вычисляет, сколько дополнительных охранников понадобится для заполнения малоохраняемых интервалов.
countAdditionalGuards :: Time -> Time -> [(Interval, Int)] -> (Int, [Interval])
countAdditionalGuards _ _ []  = error "Не заданы малоохраняемые интервалы"
countAdditionalGuards endTime watchTime ints = count watchTime ints []
    where count _ [] guards = (length guards, guards)
          count t ints guards
              | null covered = let t' = minimum $ map (start . fst) ints 
                               in count (t' +: watchTime) ints guards  
              | otherwise = let t' = t +: watchTime
                                n  = 2 - (minimum covered)
                                endt = min t endTime
                                interval = Interval (endt -: watchTime) endt
                                guards' = guards ++ replicate n interval
                                ints' = dropWhile (\i -> end (fst i) <= t) ints
                            in count t' ints' guards'
              where covered = map snd $ takeWhile (\i -> start (fst i) < t) ints

-- | Меняет график таким образом, чтобы малоохраняемых интервалов не оставалось (если это возможно) 
reorganizeSchedule :: Time -> Schedule -> Maybe Schedule
reorganizeSchedule endTime gs 
    | manHours < endTime +: endTime = Nothing
    | otherwise = 
        case find halfSchedule $ subsets gs of
            Nothing    -> Nothing
            Just first -> 
                let second = gs \\ first
                in Just $ map adjustWatch $ align (Time 0) first ++ align (Time 0) second    
    where watchSum = foldl (+:) (Time 0) . map (duration . watch)
          manHours = watchSum gs
          halfSchedule gs = watchSum gs `between` (endTime, manHours -: endTime)
          align _ [] = []
          align start (g:gs) = 
            let start' = start +: (duration $ watch g)
            in shift start g : align start' gs
          adjustWatch g@(Guard _ watch) 
            | end watch > endTime = shiftEnd endTime g
            | otherwise = g

-- | Обработка данных
lab5 :: InputData -> IO ()
lab5 (InputData endTime schedule watchLen) = do
    let weakIntervals = weakGuardIntervals endTime schedule
    case weakIntervals of 
        [] -> putStrLn "\nа) малоохраняемых интервалов нет"
        weakIntervals -> do 
            putStrLn "\nа) найдены малоохраняемые интервалы"
            putStrLn $ "\nб) " ++ show weakIntervals
            let additionalGuards = countAdditionalGuards endTime watchLen weakIntervals
            putStrLn $ "\nв) дополнительных охранников: " ++ show (fst additionalGuards)
            putStrLn $ show (snd additionalGuards)
            let newSchedule = reorganizeSchedule endTime schedule
            case newSchedule of
                Just newSchedule -> do
                    putStrLn "\nг) новое расписание: " 
                    putStrLn $ show newSchedule
                Nothing ->
                    putStrLn "\nг) обойтись без дополнительных охранников невозможно\n"
                       