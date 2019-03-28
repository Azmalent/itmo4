module Test (testData) where
import Datatypes

time :: Int -> Int -> Time
time h m = Time $ (h * 60) + m

getSchedule :: [(Time, Time)] -> Schedule
getSchedule watches = let watchIntervals = map (uncurry Interval) watches
                      in zipWith Guard [1..] watchIntervals

testData :: InputData
testData = let endTime   = time 20 00
               watches   = [(time 00 00, time 15 30), (time 00 00, time 15 30), (time 04 00, time 17 20), (time 12 30, time 18 20),
                            (time 06 00, time 11 25), (time 06 00, time 11 25)]
               watchTime = time 05 00
           in InputData endTime (getSchedule watches) watchTime