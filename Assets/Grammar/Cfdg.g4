grammar Cfdg;

contextfree : statements EOF ;

statements : statements statement | ;

statement : startshape | background | design_rule ;

startshape : STARTSHAPE NAME ;

background : BACKGROUND '{' color_adjustments '}' ;

design_rule
    : RULE NAME '{' replacements '}'
    | RULE NAME num '{' replacements '}'
    ;
    
replacements : replacement_loop* ;

replacement_loop 
    : replacement 
    | NUMBER '*' modification replacement
    | NUMBER '*' modification '{' replacements '}'
    ;

replacement : NAME modification ;

modification : '{' adjustments '}' | '[' adjustments ']' ;

adjustments : adjustment*;

adjustment : geom_adjustment | color_adjustment ;

color_adjustments : color_adjustments color_adjustment | ;

geom_adjustment
    : XSHIFT num
    | YSHIFT num
    | ZSHIFT num
    | ROTATE num
    | SIZE num
    | SIZE num num
    | SIZE num num num
    | SKEW num num
    | FLIP num
    ;

color_adjustment
    : HUE num
    | SATURATION num
    | BRIGHTNESS num
    | ALPHA num
    ;
num : NUMBER | '-' NUMBER | '+' NUMBER ;

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
NUMBER         : DIGIT+ | '.' DIGIT* | DIGIT+ '.' DIGIT* ;
NAME : [a-zA-Z_][a-zA-Z_0-9]* ;

WS : [ \t\r\n]+ -> skip ;
COMMENT : '/*' .*? '*/' -> skip ;
LINE_COMMENT : '//' ~[\r\n]* -> skip ;
LINE_BANG_COMMENT : '#' ~[\r\n]* -> skip ;
