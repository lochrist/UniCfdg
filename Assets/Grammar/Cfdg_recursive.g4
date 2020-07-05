grammar Cfdg;

contextfree : statements EOF ;

statements : statements statement | ;

statement : startshape | background | design_rule ;

startshape : STARTSHAPE NAME ;

background : BACKGROUND '{' color_adjustments '}' ;

design_rule
    : RULE NAME '{' replacements '}'
    | RULE NAME NUMBER '{' replacements '}'
    ;
    
replacements : replacements replacement_loop | ;

replacement_loop : replacement ;

replacement : NAME modification ;

modification : '{' adjustments '}' | '[' adjustments ']' ;

adjustments : adjustments adjustment | ;

adjustment : geom_adjustment | color_adjustment ;

color_adjustments : color_adjustments color_adjustment | ;

geom_adjustment
    : XSHIFT NUMBER
    | YSHIFT NUMBER
    | ZSHIFT NUMBER
    | ROTATE NUMBER
    | SIZE NUMBER
    | SIZE NUMBER NUMBER
    | SIZE NUMBER NUMBER NUMBER
    | SKEW NUMBER NUMBER
    | FLIP NUMBER
    ;

color_adjustment
    : HUE NUMBER
    | SATURATION NUMBER
    | BRIGHTNESS NUMBER
    | ALPHA NUMBER
    ;    

STARTSHAPE : 'startshape' ;
BACKGROUND : 'background' ;
RULE : 'rule' ;
ROTATE : ('rotate' | 'r') ; 
FLIP : ('flip' | 'f') ;
HUE : ('hue' | 'h') ;
SATURATION : ('saturation' | 'sat') ;
BRIGHTNESS : ('brightness' | 'b') ;
ALPHA : ('alpha' | 'a') ;
XSHIFT : 'x' ;
YSHIFT : 'y' ;
ZSHIFT : 'z' ;
SIZE : ('size' | 's') ;
SKEW : 'skew' ;

fragment DIGIT : [0-9] ;
NUMBER         : DIGIT+ ([.,] DIGIT+)? ;
NAME : [a-zA-Z_][a-zA-Z_0-9]* ;

WS : [ \t\r\n]+ -> skip ;
