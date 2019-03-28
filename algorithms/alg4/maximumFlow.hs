{-# LANGUAGE TupleSections #-}

import Data.Maybe (fromJust)
import Data.List ((\\), find, union, partition)
import Control.Exception (assert)
import Utils
import DataTypes

--Наша транспортная сеть
network :: Network
network = let arcData = [('A', 'B', 3), ('A', 'C', 6), ('A', 'F', 7), ('B', 'D', 4), ('B', 'C', 2), ('C', 'E', 3), 
                         ('D', 'B', 4), ('D', 'F', 6), ('E', 'C', 3), ('E', 'D', 1), ('E', 'F', 5)]
              e = [Arc (Node s) (Node d) c | (s, d, c) <- arcData]
           in Network (Node 'A') (Node 'F') e

--Возвращает всех соседей узла, считая граф неориентированным.
neighbours :: Network -> Node -> [Node]
neighbours (Network _ _ arcs) v = let dests   = [t | (Arc s t _) <- arcs, s == v]
                                      sources = [s | (Arc s t _) <- arcs, t == v]
                                  in dests `union` sources
                 
--Находит все пути из истока в сток, считая граф неориентированным.
nodePaths :: Network -> [[Node]]
nodePaths g@(Network src sink _) = pathsFrom src [src]
    where pathsFrom v p | v == sink      = [p]
                        | null nextNodes = []  
                        | otherwise      = concatMap (\w -> pathsFrom w $ p ++ [w]) nextNodes
                        where nextNodes  = neighbours g v \\ p

--Находит цепи, помечая направление каждой дуги.
arcPaths :: Network -> [Path]
arcPaths g = concatMap mark $ nodePaths g
    where mark p = let arcPaths = cartesian $ zipWith arcsBetween p (tail p)
                   in map (\path -> zipWith markDirection p path) arcPaths
          markDirection v arc@(Arc s _ _) = (arc, v == s)
          arcsBetween u v = filter (\(Arc s t _) -> (s, t) == (u, v) || (s, t) == (v, u)) $ netArcs g

--Находит максимальный поток транспортной сети.
maximumFlow :: Network -> Int
maximumFlow g = let arcFlows = map (, 0) $ netArcs g
                in maximumFlow' arcFlows (arcPaths g) 0
    where maximumFlow' flows paths maxFlow =
            case find (all allowed) paths of
                Just path -> 
                    let path' = map fst path
                        sigma = minimum [arcCap arc - flow | (arc, flow) <- flows, arc `elem` path']
                    in maximumFlow' (update flows path sigma) paths (maxFlow + sigma)
                Nothing -> 
                    let minCut = minimumCut g flows
                    in assert (minCut == maxFlow) maxFlow
            where getFlow arc = fromJust $ lookup arc flows
                  allowed (arc, True)  = getFlow arc < arcCap arc
                  allowed (arc, False) = getFlow arc > 0
          update flows path sigma = let (plusArcs, minusArcs) = mapPair fst $ partition snd path  
                                        update' pair@(arc, flow) 
                                            | arc `elem` plusArcs  = (arc, flow + sigma)
                                            | arc `elem` minusArcs = (arc, flow - sigma)
                                            | otherwise = pair
                                    in map update' flows

--Находит минимальный разрез транспортной сети.
minimumCut :: Network -> [ArcFlow] -> Int
minimumCut (Network src _ _) flows = sum [flow | (arc, flow) <- flows, shouldCut arc]
    where unsaturated = map fst $ filter (\(Arc _ _ c, f) -> f < c) flows
          marked = findMarkedNodes src [src]
          findMarkedNodes v m 
              | null nextNodes = [v]
              | otherwise = v : concatMap (\w -> findMarkedNodes w $ w:m) nextNodes
              where nextNodes = [t | (Arc s t _) <- unsaturated, s == v] \\ m
          shouldCut arc = connectsSets arc && arc `notElem` unsaturated
          connectsSets (Arc s t _) = (s `elem` marked) /= (t `elem` marked)
