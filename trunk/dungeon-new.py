import random

DEBUG = True

if DEBUG:
    def center(i, width):
        if i >= 10**width:
            raise OverflowError

        ret = ""
        leftside = ""
        rightside = ""
        rightsize = 0
        ilen = len(str(i))
        ret = str(i)
        rightsize = (width - ilen) / 2
        leftside = " " * (width - rightsize - ilen)
        rightside = " " * rightsize
        ret = leftside + ret + rightside
        return ret

def slope(p1, p2, x2 = None, y2 = None):
    if y2 == None:
        if x2 != None:
            raise TypeError, "slope() requires 2 2-tuples or 4 numbers"
        else:
            if len(x2) != 2 or len(y2) != 2:
                raise TypeError, "slope() requires 2 2-tuples or 4 numbers"
            else:
                x1, y1 = p1
                x2, y2 = p2
    else:
        x1 = p1
        y1 = p2

    if x2 == x1:
        if y2-y1 > 0:
            return "inf"
        else:
            return "ninf"
    else:
        return (y2-y1) / (x2-x1)

class Player:
    def __init__(self):
        self.row = 2
        self.col = 3

class Direction:
    def __init__(self, name, deltar, deltac, opposite):
        self.name = name
        self.deltar = deltar
        self.deltac = deltac
        self.opposite = opposite

EXITS = {}
EXITS['n'] = Direction('n',-1,0,'s')
EXITS['e'] = Direction('e',0,1,'w')
EXITS['s'] = Direction('s',1,0,'n')
EXITS['w'] = Direction('w',0,-1,'e')

EXITORDER = ['n','e','s','w']

class Room:
    id = 0
    def __init__(self):
        self.exits = {}
        for exit in EXITORDER:
            self.exits[exit] = -1
        self.visited = 0
        self.id = Room.id
        Room.id += 1

class Maze:
    def __init__(self, rows, cols,player):
        self.rows = rows
        self.cols = cols
        self.maze = []
        self.player = player
        
    def newmaze(self):
        self.maze = []
        for r in range(self.rows):
            row = []
            for c in range(self.cols):
                row.append(Room())
            self.maze.append(row)

    def validCoord(self,c):
        retval = True
        if len(c) != 2:
            retval = False
        if c[0] < 0 or c[0] >= self.rows:
            retval = False
        if c[1] < 0 or c[1] >= self.cols:
            retval = False
        return retval

    def __getitem__(self,c):
        if type(c) == int:
            return self.maze[c]
        elif len(c) == 2:
            if c[0] >= 0 and c[0] < self.rows and c[1] >= 0 and c[1] < self.cols:
                return self.maze[c[0]][c[1]]
            else:
                return None
        else:
            raise TypeError

    def generate(self):
        self.newmaze()
        r = random.randint(0,self.rows-1)
        c = random.randint(0,self.cols-1)

        if DEBUG:
            print r,c
        
        self.maze[r][c].visited = 1
        visitedStack = []
        visited = 1
        while visited < self.rows*self.cols:
            adjacentRooms = []

            for ex in EXITORDER:
                if self[r+EXITS[ex].deltar, c+EXITS[ex].deltac] != None and \
                   self[r+EXITS[ex].deltar, c+EXITS[ex].deltac].visited == False:
                    adjacentRooms.append(ex)
                    
            if len(adjacentRooms) > 0:
                direction = random.choice(adjacentRooms)
                nextR = r + EXITS[direction].deltar
                nextC = c + EXITS[direction].deltac
                oppDir = EXITS[direction].opposite
                self[r,c].exits[direction] = self[nextR,nextC].id
                self[nextR,nextC].exits[oppDir] = self[r,c].id
                self[nextR,nextC].visited = True
                visitedStack.append((r,c))
                r,c = nextR, nextC
                visited += 1
            else:
                r,c = visitedStack.pop()

    def generateVisibleGrid(self,visibleGrid,r,c,pr,pc,direction,dist,maxDist):
        if dist != maxDist:
            visibleGrid[r][c] = 1
            for ex in EXITORDER:
                if self[pr+r,pc+c].exits[ex] != -1 and \
                   visibleGrid[r+EXITS[ex].deltar][c+EXITS[ex].deltac] == 0 and\
                   ex != EXITS[direction].opposite:
                    self.generateVisibleGrid(visibleGrid,r+EXITS[ex].deltar,\
                                             c+EXITS[ex].deltac,pr,pc,direction,\
                                             dist+1,maxDist)
        else:
            visibleGrid[r][c] = 2

    def displayMap(self,pr,pc,light):
        ''' Display's player's map of the area around him.
        Displays current room at light 0, displays one additional
        room around for each light level above 1.
        Symbols:
        @: Player
        !: Monster heard (on edge of light range)
        $: Loot
        n: Monster. Any alphabet letter.
        '''

        visibleGrid = []
        visiblerows = visiblecols = light * 2 + 1
        for r in range(visiblerows):
            row = []
            for c in range(visiblecols):
                row.append(0)
            visibleGrid.append(row)

        visibleGrid[0][0] = 1
        if light > 0:
            for ex in EXITORDER:
                if self[pr,pc].exits[ex] != -1:
                    self.generateVisibleGrid(visibleGrid,EXITS[ex].deltar,\
                                             EXITS[ex].deltac,pr,pc,ex,1,light)

        startr = -light
        finishr = light+1

        while visibleGrid[startr] == [0] * visiblerows:
            visibleGrid.pop(startr)
            startr += 1

        while visibleGrid[finishr-1] == [0] * visiblerows:
            visibleGrid.pop(finishr-1)
            finishr -= 1

        startc = -light
        finishc = light+1

        if DEBUG:
            print visibleGrid

        finished = 0
        for r in range(len(visibleGrid)):
            if visibleGrid[r][startc] != 0:
                finished = 1
                break

        while not finished:
            if DEBUG: print startc
            for r in range(len(visibleGrid)):
                visibleGrid[r].pop(startc)
            startc += 1

            finished = 0
            for r in range(len(visibleGrid)):
                if visibleGrid[r][startc] != 0:
                    finished = 1
                    break

        if DEBUG:
            print visibleGrid

        finished = 0
        for r in range(len(visibleGrid)):
            if visibleGrid[r][finishc-1] != 0:
                finished = 1
                break

        while not finished:
            if DEBUG:
                print finishc
            for r in range(len(visibleGrid)):
                visibleGrid[r].pop(finishc-1)
            finishc -= 1

            finished = 0
            for r in range(len(visibleGrid)):
                if visibleGrid[r][finishc-1] != 0:
                    finished = 1
                    break

        if DEBUG:
            print visibleGrid

        yield "*" * (2+(abs(startc)+finishc)*5)
        for row in range(startr,finishr):
            out = ""
            for col in range(startc,finishc):
                if visibleGrid[row][col] == 0:
                    out += " " * 5
                else:
                    out += "+-"
                    if self[pr+row,pc+col].exits['n'] != -1:
                        out += " "
                    else:
                        out += "-"
                    out += "-+"
            yield "*"+out+"*"

            out = ""
            for col in range(startc,finishc):
                if visibleGrid[row][col] == 0:
                    out += " " * 5
                else:
                    if self[pr+row, pc+col].exits['w'] != -1:
                        out += "  "
                    else:
                        out += "| "
                    if row == 0 and col == 0:
                        out += "@"
                    else:
                        out += " "
                    if self[pr+row, pc+col].exits['e'] != -1:
                        out += "  "
                    else:
                        out += " |"
            yield "*"+out+"*"

            out = ""
            for col in range(startc,finishc):
                if visibleGrid[row][col] == 0:
                    out += " " * 5
                else:
                    out += "+-"
                    if self[pr+row,pc+col].exits['s'] != -1:
                        out += " "
                    else:
                        out += "-"
                    out += "-+"
            yield "*"+out+"*"

        yield "*" * (2+(abs(startc)+finishc)*5)

    if DEBUG:
        def display(self):
            for r in range(self.rows):
                out = ""
                for c in range(self.cols):
                    out += "+-"
                    if self[r,c].exits['n'] == -1:
                        out += "-"
                    else:
                        out += " "
                    out += "-+"
                print out

#                out = ""
#                for c in range(self.cols):
#                    out += "|" + " " * 9 + "|"
#                print out

                out = ""
                for c in range(self.cols):
                    if self[r,c].exits['w'] == -1:
                        out += "| "
                    else:
                        out += "  "
                    if r == self.player.row and c == self.player.col:
                        out += "@"
                    else:
                        out += " "
                    if self[r,c].exits['e'] == -1:
                        out += " |"
                    else:
                        out += "  "
                print out

#                out = ""
#                for c in range(self.cols):
#                    out += "|" + " " * 9 + "|"
#                print out

                out = ""
                for c in range(self.cols):
                    out += "+-"
                    if self[r,c].exits['s'] == -1:
                        out += "-"
                    else:
                        out += " "
                    out += "-+"
                print out

p = Player()
m = Maze(7,9,p)
m.generate()
m.display()
for row in m.displayMap(2,3,4):
    print row
