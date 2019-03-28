fact :: Integer -> Integer
fact 0 = 1
fact n | n > 0  = n * fact (n - 1)
       | otherwise = undefined

dfact :: Integer -> Integer
dfact 0 = 1
dfact 1 = 1
dfact n | n > 1  = n * dfact(n - 2)
        | otherwise = undefined
        
fib :: Integer -> Integer
fib 0 = 0
fib 1 = 1
fib n | n > 1 = fib (n - 1) + fib (n - 2)
      | otherwise = fib (n + 2) - fib (n + 1)

--Рекурсия с явным аккумулятором
--Даёт линейное количество вызовов от n
fib' :: Integer -> Integer
fib' 0 = 0
fib' 1 = 1
fib' n | n > 1 = let helper fib2 fib1 2 = (fib2 + fib1)  
                     helper fib2 fib1 n = helper fib1 (fib2 + fib1) (n - 1) 
                  in helper 0 1 n
       | otherwise = let helper' fib1 fib2 (-1) = (fib2 - fib1)
                         helper' fib1 fib2 n  = helper' (fib2 - fib1) fib1 (n + 1)
                     in helper' 0 1 n