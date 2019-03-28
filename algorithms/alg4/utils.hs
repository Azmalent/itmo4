module Utils where

cartesian :: [[a]] -> [[a]]
cartesian [x] = [x]
cartesian [x, y] = [[a, b] | a <- x, b <- y]
cartesian (x:xs) = [y:ys | y <- x, ys <- cartesian xs]

mapPair :: (a -> b) -> ([a], [a]) -> ([b], [b])
mapPair f (a, b) = (map f a, map f b)