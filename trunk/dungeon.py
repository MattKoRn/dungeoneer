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

class Coord:
    def __init__(self, row, col):
        self.row = row
        self.col = col
        self.coord = (row,col)

    def __getitem__(self, item):
        if item != 1 and item != 0:
            raise IndexError

        return self.coord[item]

    def __add__(self, other):
        if other.__class__ != Coord:
            raise TypeError, "Can't add types coord and " + str(other.__class__)
        return Coord(self.row + other.row, self.col + other.col)

    def __mul__(self, other):
        if type(other) != int:
            raise TypeError, "Can't multiply types coord and " + str(other.__class__)
        return Coord(self.row * other, self.col * other)

    def __str__(self):
        return "(" + str(self.row) + "," + str(self.col) + ")"

class Player:
    def __init__(self):
        self.loc = Coord(2,3)

class Direction:
    def __init__(self, name, deltar, deltac, opposite):
        self.name = name
        self.deltar = deltar
        self.deltac = deltac
        self.opposite = opposite
        self.delta = Coord(deltar, deltac)

    def coordMove(self, coord, dist):
        return coord + self.delta * dist

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
        for ex in EXITORDER:
            self.exits[ex] = -1
        self.visited = 0
        self.id = Room.id
        Room.id += 1

    def see_items(self):
        return random.random() > .5

    def hear_monsters(self):
        return random.random() > .5

class Grid:
    def __init__(self, rows, cols):
        self.rows = rows
        self.cols = cols
        self.grid = []
        self.wraparound = True
        for r in range(rows):
            row = []
            for c in range(cols):
                row.append(0)
            self.grid.append(row)

    def validCoord(self,c):
        retval = False
        if self.wraparound:
            lowerRowLimit = (self.rows) * -1
            lowerColLimit = (self.cols) * -1
        else:
            lowerRowLimit = lowerColLimit = 0
        if c[0] >= lowerRowLimit and\
           c[0] < self.rows and\
           c[1] >= lowerColLimit and\
           c[1] < self.cols:
            retval = True
        return retval

    def __getitem__(self,c):
        if c.__class__ == int:
            return self.grid[c]
        elif c.__class__ == Coord:
            if self.validCoord(c):
                return self.grid[c[0]][c[1]]
            else:
                return None
        elif len(c) == 2:
            if self.wraparound:
                lowerRowLimit = (self.rows) * -1
                lowerColLimit = (self.cols) * -1
            else:
                lowerRowLimit = lowerColLimit = 0
            if c[0] >= lowerRowLimit and\
               c[0] < self.rows and\
               c[1] >= lowerColLimit and\
               c[1] < self.cols:
                return self.grid[c[0]][c[1]]
            else:
                return None
        else:
            raise TypeError

    def __setitem__(self,c,val):
        if c.__class__ == Coord:
            if self.validCoord(c):
                self.grid[c[0]][c[1]] = val
        elif len(c) == 2:
            if self.wraparound:
                lowerRowLimit = (self.rows) * -1
                lowerColLimit = (self.cols) * -1
            else:
                lowerRowLimit = lowerColLimit = 0
            if c[0] >= lowerRowLimit and\
               c[0] < self.rows and\
               c[1] >= lowerColLimit and\
               c[1] < self.cols:
                self.grid[c[0]][c[1]] = val
        else:
            raise TypeError

    def pop(self, row):
        self.grid.pop(row)

    def __len__(self):
        return len(self.grid)

class Maze(Grid):
    def __init__(self, rows, cols,player):
        Grid.__init__(self,rows,cols)
        self.maze = self.grid
        self.player = player
        self.wraparound = False
        
    def newmaze(self):
        self.maze = []
        self.grid = self.maze
        for r in range(self.rows):
            row = []
            for c in range(self.cols):
                row.append(Room())
            self.maze.append(row)

    def validCoord(self,c):
        retval = True
        if c[0] < 0 or c[0] >= self.rows:
            retval = False
        if c[1] < 0 or c[1] >= self.cols:
            retval = False
        return retval

    def generate(self):
        self.newmaze()
        c = Coord(random.randint(0,self.rows-1),random.randint(0,self.cols-1))

        if DEBUG:
            print c
        
        self[c].visited = 1
        visitedStack = []
        visited = 1
        while visited < self.rows*self.cols:
            adjacentRooms = []

            for ex in EXITORDER:
                if self[c+EXITS[ex].delta] != None and \
                   self[c+EXITS[ex].delta].visited == False:
                    adjacentRooms.append(ex)
                    
            if len(adjacentRooms) > 0:
                direction = random.choice(adjacentRooms)
                nextC = c + EXITS[direction].delta
                oppDir = EXITS[direction].opposite
                self[c].exits[direction] = self[nextC].id
                self[nextC].exits[oppDir] = self[c].id
                self[nextC].visited = True
                visitedStack.append(c)
                c = nextC
                visited += 1
            else:
                c = visitedStack.pop()

    def displayMap(self,pc,light):
        ''' Display's player's map of the area around him.
        Displays current room at light 0, displays one additional
        room around for each light level above 1.
        Symbols:
        @: Player
        !: Monster heard (on edge of light range)
        $: Loot
        n: Monster. Any alphabet letter.
        '''

        visiblerows = visiblecols = light * 2 + 1
        visibleGrid = Grid(visiblerows, visiblecols)

        SEE_ROOM = 1
        SEE_THINGS = 2
        HEAR_MONSTER = 4

        visibleGrid[0][0] += SEE_ROOM

        for ex in EXITORDER:
            dist = 0
            delta = EXITS[ex].delta
            while dist+1 < light:
                if self[pc + delta].exits[ex] != -1:
                    dist += 1
                    delta = EXITS[ex].coordMove(Coord(0,0),dist)
                    visibleGrid[delta] += SEE_ROOM
                    if self[pc+delta].see_items():
                        visibleGrid[delta] += SEE_THINGS
                    if self[pc+delta].hear_monsters():
                        visibleGrid[delta] += HEAR_MONSTER

                    # Now listen for monsters to the left and right
                    if dist+1 <= light:
                        for ex2 in EXITORDER:
                            if ex2 != ex and ex2 != EXITS[ex].opposite and\
                               self[pc + delta].exits[ex2] != -1 and\
                               visibleGrid[delta + EXITS[ex2].delta] == 0:
                                if self[pc + delta + EXITS[ex2].delta].hear_monsters():
                                    visibleGrid[delta + EXITS[ex2].delta] += HEAR_MONSTER
                else:
                    break
            if self[pc+delta].exits[ex] != -1 and \
               self[pc+delta+EXITS[ex].delta].hear_monsters():
                visibleGrid[delta + EXITS[ex].delta] += HEAR_MONSTER
                
                    
                    
        

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
                c = Coord(row,col)
                if visibleGrid[row][col] == 0:
                    out += " " * 5
                else:
                    out += "+-"
                    if self[pc+c].exits['n'] != -1:
                        out += " "
                    else:
                        out += "-"
                    out += "-+"
            yield "*"+out+"*"

            out = ""
            for col in range(startc,finishc):
                c = Coord(row,col)
                if visibleGrid[row][col] == 0:
                    out += " " * 5
                elif visibleGrid[row][col] == HEAR_MONSTER:
                    out += "  !  "
                else:
                    if self[pc + c].exits['w'] != -1:
                        out += " "
                    else:
                        out += "|"
                    if visibleGrid[row][col] & SEE_THINGS:
                        out += "$"
                    else:
                        out += " "
                    if row == 0 and col == 0:
                        out += "@"
                    elif visibleGrid[row][col] & HEAR_MONSTER:
                        out += "!"
                    else:
                        out += " "
                    if self[pc+c].exits['e'] != -1:
                        out += "  "
                    else:
                        out += " |"
            yield "*"+out+"*"

            out = ""
            for col in range(startc,finishc):
                c = Coord(row, col)
                if visibleGrid[row][col] == 0:
                    out += " " * 5
                else:
                    out += "+-"
                    if self[pc+c].exits['s'] != -1:
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
                    if r == self.player.loc.row and c == self.player.loc.col:
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
m = Maze(5,7,p)
m.generate()
m.display()
for row in m.displayMap(Coord(2,3),4):
    print row
