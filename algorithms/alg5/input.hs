module Input where
import Datatypes
import Text.Read (readMaybe)

readInt :: String -> IO (Int)
readInt message = do
    putStrLn message
    input <- getLine
    let value = readMaybe input :: Maybe Int
    case value of
        Nothing -> fail "Число введено некорректно"
        Just val -> return val

readSchedule :: IO (Schedule)
readSchedule = do
    guardCount <- readInt $ "Введите количество охранников:"
    readSchedule' 1 guardCount []
    where readSchedule' i n guards = do
            case (i > n) of
                True -> return $ reverse guards
                False -> do
                    startTime' <- readInt $ "Начало дежурства сторожа №" ++ (show i) ++ ":"
                    endTime'   <- readInt $ "Конец дежурства сторожа №"  ++ (show i) ++ ":"
                    let startTime = Time startTime'
                    let endTime   = Time endTime'
                    let guard     = Guard i (Interval startTime endTime)
                    readSchedule' (i + 1) n (guard:guards)

getInputData :: IO (InputData)
getInputData = do 
    endMinutes  <- readInt "Введите время окончания стражи (в минутах):"
    schedule <- readSchedule
    watchMinutes <- readInt "Введите длительность дежурства дополнительного сторожа (в минутах):"
    let endTime = Time endMinutes
    let watchTime = Time watchMinutes
    return $ InputData endTime schedule watchTime