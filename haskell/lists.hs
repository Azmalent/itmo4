foldl' :: (b -> a -> b) -> b -> [a] -> b
foldl' _ ini [] = ini
foldl' f ini (x:xs) = foldl' f (ini `f` x) xs

foldr' :: (a -> b -> b) -> b -> [a] -> b
foldr' _ ini [] = ini
foldr' f ini (x:xs) = x `f` (foldr' f ini xs)

any' :: (a -> Bool) -> [a] -> Bool
any' pred = foldl (||) False . map pred