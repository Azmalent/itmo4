import Data.List (nub)

--С помощью функций concat и map реализуйте генерацию всех возможных перестановок элементов списка. Входной список содержит уникальные элементы.
perms :: Eq a => [a] -> [[a]]
perms [] = []
perms [x] = [[x]]
perms xs = nub $ concatMap perms' $ rotations xs                                   
    where perms' (x:xs) = [x:perm | perm <- perms xs]   -- Все перестановки, начинающиеся с x
          rotations = take (length xs) . iterate flip   -- Ротации списка: [1,2,3] -> [ [1,2,3], [2,3,1], [3,1,2] ]
          flip (x:xs) = xs ++ [x]                       -- Поменять местами голову и хвост

--Реализуйте строгую версию левой свёртки списка с помощью оператора seq. 
foldls :: (a -> b -> a) -> a -> [b] -> a
foldls _ acc [] = acc
foldls f acc (x:xs) = let acc' = acc `f` x
                      in acc' `seq` foldls f acc' xs 

--За один проход свёртки вычислить и сумму, и произведение элементов списка.
sumAndProduct :: Num a => [a] -> (a, a)
sumAndProduct = foldl (\(s, p) n -> (s + n, p * n)) (0, 1)