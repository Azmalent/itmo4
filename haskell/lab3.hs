import Data.List (nub)

--Реализуйте перестановки из #1 лабораторной с помощью монады списка (оператором монадического связывания, либо через do-нотацию).
perms :: Eq a => [a] -> [[a]]
perms [] = []
perms [x] = [[x]]
perms xs = nub $ do let len = length xs
                    let flip (x:xs) = xs ++ [x]
                    rotation <- take len $ iterate flip xs             --Ротации списка
                    (y:ys)   <- return rotation                        --Разделяем каждую ротацию списка на голову и хвост
                    result   <- do tailPerm <- perms ys                --Голову соединяем с каждой перестановкой хвоста
                                   return $ y : tailPerm
                    return result

--Объявите частично примененную функциональную стрелку представителем класса Monad. 
--В качестве побочного эффекта реализация должна “предоставлять доступ к одному и тому же внешнему содержимому” для цепочки монадических вычислений.
data MyReader env a = MyReader { getFunc :: env -> a }

instance Functor (MyReader env) where
      fmap f (MyReader g) = MyReader (f . g)

instance Applicative (MyReader env) where
      MyReader f <*> MyReader g = MyReader (\e -> f e $ g e)
      pure = MyReader . const

instance Monad (MyReader env) where
      MyReader f >>= g = MyReader (\e -> let u = getFunc $ (g . f) e in u e)

--Объявите пару (кортеж из двух элементов) представителем класса Monad. 
--В качестве побочного эффекта реализация должна “накапливать содержимое” по одному из элементов пары в цепочке монадических вычислений. 
--Для этого элемента следует наложить контекст Monoid.
data Pair m a = Pair { unpair :: (a, m) } deriving (Show, Eq)

instance (Monoid m) => Functor (Pair m) where
      fmap f (Pair (a, m)) = Pair (f a, m)

instance (Monoid m) => Applicative (Pair m) where
      Pair (f, m) <*> Pair (a, n) = Pair (f a, m `mappend` n)
      pure a = Pair (a, mempty)

instance (Monoid m) => Monad (Pair m) where
      Pair (a, m) >>= f = let (b, n) = unpair $ f a
                          in Pair (b, m `mappend` n)