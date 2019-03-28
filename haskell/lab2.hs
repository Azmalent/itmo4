import Data.Foldable (toList)
import RingMod

--Определите тип данных Двоичное дерево.
data Tree a = Node { value :: a, left :: Tree a, right :: Tree a } | Null deriving (Show, Eq)

instance Foldable Tree where
    foldMap = postorder

tree = Node 5 (Node 3 (Node 0 Null Null) (Node 2 Null Null)) (Node 6 (Node 11 Null Null) Null)

dfs :: Monoid m => (a -> m) -> Tree a -> m
dfs _ Null = mempty
dfs f (Node x l r) = f x `mappend` dfs f l `mappend` dfs f r

bfs :: Monoid m => (a -> m) -> Tree a -> m
bfs _ Null = mempty
bfs f t = bfs' mempty [t]
    where bfs' acc [] = acc
          bfs' acc (t:ts) = let acc' = acc `mappend` (f $ value t) 
                            in bfs' acc' $ ts ++ subtrees t
          subtrees t = [ sub | sub@(Node _ _ _) <- [left t, right t] ]

preorder :: Monoid m => (a -> m) -> Tree a -> m
preorder _ Null = mempty
preorder f (Node x l r) = f x `mappend` preorder f l `mappend` preorder f r

inorder :: Monoid m => (a -> m) -> Tree a -> m
inorder _ Null = mempty
inorder f (Node x l r) = inorder f l `mappend` f x `mappend` inorder f r

postorder :: Monoid m => (a -> m) -> Tree a -> m
postorder _ Null = mempty
postorder f (Node x l r) = postorder f l `mappend` postorder f r `mappend` f x