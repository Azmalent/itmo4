module DataTypes where

newtype Node = Node { nodeLabel :: Char } deriving Eq

instance Show Node where
      show = show . nodeLabel
    
data Arc = Arc { arcSrc :: Node, arcDest :: Node, arcCap :: Int } deriving Eq

instance Show Arc where
    show (Arc src dest cap) = nodeLabel src : nodeLabel dest : " " ++ show cap
    
data Network = Network { netSrc :: Node, netSink :: Node, netArcs :: [Arc] } deriving Show

type ArcFlow = (Arc, Int)
type Path = [(Arc, Bool)]