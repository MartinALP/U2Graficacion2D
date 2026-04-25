# Unidad 2 — Graficación 2D
## Proyecto de ejemplos interactivos en C# / WinForms (.NET 9)

> Cada tema de la unidad está en su propio `TabPage`.
> Ejecuta el proyecto con `dotnet run` desde la carpeta `U2Graficacion2D/`.

---

## 2.1 Transformaciones Bidimensionales

Las transformaciones 2D permiten cambiar la posición, tamaño u orientación de un objeto
sin alterar su forma intrínseca. Se aplican a cada vértice del objeto.

---

### 2.1.1 Traslación — `TabTraslacion.cs`

**Concepto:**  
Mover un objeto de un lugar a otro sumando un vector de desplazamiento `(tx, ty)` a cada punto.

```
x' = x + tx
y' = y + ty
```

**Lo que muestra el ejemplo:**
- Triángulo gris = posición original.
- Triángulo azul = posición trasladada.
- Flecha roja = vector de traslación (tx, ty).
- Los sliders permiten cambiar `tx` y `ty` en tiempo real (−300 a 300 px).

**Ejercicio para el estudiante — Práctica 2.1:**
> Modifica el programa para que el triángulo se **traslade en bucle de forma animática** 
> (usando un `Timer`): que vaya desde el origen hasta (200, 150) y regrese, 
> de manera suave, usando incrementos de 2 px por fotograma.
> Agrega un botón "Iniciar / Detener" para controlar la animación.

---

### 2.1.2 Escalamiento — `TabEscalamiento.cs`

**Concepto:**  
Cambiar el tamaño de un objeto multiplicando cada coordenada por factores de escala `sx` y `sy`.

```
x' = sx · x
y' = sy · y
```

- `sx = sy > 1` → agranda uniformemente  
- `0 < sx = sy < 1` → reduce  
- `sx ≠ sy` → escala no uniforme (distorsión)

> El escalamiento aquí es **respecto al origen**. Si se desea escalar respecto a un 
> punto pivote `(px, py)`, la secuencia es:  
> trasladar `(-px,-py)` → escalar → trasladar `(px,py)`.

**Lo que muestra el ejemplo:**
- Triángulo gris = original. Triángulo verde = escalado.
- Sliders `sx` y `sy` independientes (0.1× a 4.0×).

**Ejercicio para el estudiante — Práctica 2.1:**
> Implementa **escalamiento con pivote**: agrega dos sliders más para el punto 
> pivote `(px, py)`, aplica la secuencia traslación-escala-traslación inversa y 
> dibuja un marcador en el pivote. Verifica que el punto pivote no se mueva al escalar.

---

### 2.1.3 Rotación — `TabRotacion.cs`

**Concepto:**  
Girar un objeto un ángulo θ en sentido horario o anti-horario alrededor del origen.

```
x' = x·cos(θ) − y·sin(θ)
y' = x·sin(θ) + y·cos(θ)
```

> Para rotar respecto a un punto `(cx, cy)` que no sea el origen:  
> trasladar `(-cx,-cy)` → rotar → trasladar `(cx,cy)`.

**Lo que muestra el ejemplo:**
- Triángulo gris = original. Triángulo morado = rotado.
- Arco naranja que indica visualmente el ángulo θ.
- Slider de 0° a 360°.

**Ejercicio para el estudiante — Práctica 2.1:**
> Agrega un segundo slider para seleccionar el **centro de rotación** (x y y del pivote), 
> dibuja el pivote como un círculo rojo y muestra cómo la figura gira alrededor de ese punto.
> Compara el resultado cuando el pivote está en el centro del triángulo vs. en una esquina.

---

### 2.1.4 Sesgado (Shear) — `TabSesgado.cs`

**Concepto:**  
Inclinar el objeto en dirección X o Y. Un punto se desplaza proporcionalmente a su
coordenada perpendicular.

```
Sesgado en X:   x' = x + shx·y     y' = y
Sesgado en Y:   x' = x             y' = y + shy·x
```

Se usa en efectos de sombra, perspectiva itálica y deformaciones en fuentes.

**Lo que muestra el ejemplo:**
- Rectángulo gris = original. Rectángulo naranja = sesgado.
- Sliders independientes para `shx` y `shy` (−2.0 a 2.0).

**Ejercicio para el estudiante — Práctica 2.1:**
> Reemplaza el rectángulo por una figura con más vértices (estrella de 5 puntas) 
> y aplica el sesgado. Después, agrega un **tercer slider de ángulo** que convierta 
> el ángulo deseado en el factor `shx = tan(ángulo)` para que el usuario manipule 
> el sesgado en grados en lugar de factor directo.

---

## 2.2 Representación Matricial — `TabMatrices.cs`

**Concepto:**  
Usando **coordenadas homogéneas** se agrega una tercera dimensión ficticia `w=1` 
al vector de punto `[x, y, 1]^T`. Esto permite expresar **todas** las transformaciones 
(incluida la traslación) como multiplicaciones de matrices 3×3.

```
Traslación:         Escalamiento:        Rotación (θ):
[1  0  tx]          [sx  0  0]           [cos θ  −sin θ  0]
[0  1  ty]          [0  sy  0]           [sin θ   cos θ  0]
[0  0   1]          [0   0  1]           [0       0      1]
```

La **composición** de transformaciones se logra multiplicando matrices en orden:

```
M_compuesta = T · S · R
Resultado   = M_compuesta · [x, y, 1]^T
```

**Lo que muestra el ejemplo:**
- Panel derecho: muestra cada matriz individual y la compuesta `T·S·R` en tiempo real.
- Panel de dibujo: resultado visual de la transformación compuesta.
- Sliders para `tx, ty, sx, sy, θ`.

**Ejercicio para el estudiante — Práctica 2.1:**
> Agrega un **ComboBox de orden de transformaciones** con opciones: 
> `T→S→R`, `R→S→T`, `S→T→R`, etc.  
> Compara los resultados y explica **por qué el orden de multiplicación importa** 
> (las matrices no son conmutativas). Incluye al menos 3 capturas de pantalla 
> con el mismo `tx=100, sx=2, θ=45°` y diferentes órdenes.

---

## 2.3 Trazo de Líneas Curvas

### 2.3.1 Curvas de Bézier — `TabBezier.cs`

**Concepto:**  
Una curva de Bézier de grado `n` se define con `n+1` puntos de control usando los 
**polinomios de Bernstein**:

$$B(t) = \sum_{i=0}^{n} \binom{n}{i}(1-t)^{n-i} t^i \cdot P_i \quad t \in [0,1]$$

La **cúbica** (grado 3, 4 puntos) es la más usada:

$$B(t) = (1-t)^3 P_0 + 3(1-t)^2 t\, P_1 + 3(1-t)t^2 P_2 + t^3 P_3$$

**Propiedades clave:**
- La curva comienza en P0 y termina en P3 (interpolación en extremos).
- P1 y P2 son "imanes" que atraen la curva (no la tocan).
- Convex hull: la curva está dentro del polígono de control.

**Lo que muestra el ejemplo:**
- 4 puntos de control arrastrables con el ratón.
- Polígono de control en línea punteada azul.
- Curva en azul oscuro, trazada evaluando `B(t)` con 300 pasos.
- Punto especial en `t = 0.5` marcado en rojo.

**Ejercicio para el estudiante — Práctica 2.1:**
> Convierte el programa para manejar **Bézier de grado variable** (quadrática = 3 pts, 
> cúbica = 4 pts, cuártica = 5 pts): agrega botones "+" y "−" para agregar/quitar puntos 
> de control y generaliza el algoritmo de Bernstein (o implementa el **algoritmo de 
> De Casteljau** de forma recursiva). Dibuja los pasos intermedios de De Casteljau 
> animados con un slider `t`.

---

### 2.3.2 B-Spline — `TabBSpline.cs`

**Concepto:**  
Las B-Splines extienden Bézier con **control local**: mover un punto solo afecta los 
segmentos vecinos, no toda la curva. La curva es suave (`C²` continua).

Para una **B-Spline cúbica uniforme**, cada segmento `i` usa 4 puntos 
`P_i, P_{i+1}, P_{i+2}, P_{i+3}` con funciones base:

```
b₀(t) = (−t³ + 3t² − 3t + 1) / 6
b₁(t) = (3t³ − 6t² + 4) / 6
b₂(t) = (−3t³ + 3t² + 3t + 1) / 6
b₃(t) = t³ / 6
```

**Diferencias clave vs. Bézier:**

| | Bézier | B-Spline |
|---|---|---|
| Interpolación | Solo extremos | Ninguno (aproximación) |
| Control | Global | Local |
| Continuidad | C¹ (cúbica) | C² (cúbica) |
| N puntos | 1 curva | N-3 segmentos |

**Lo que muestra el ejemplo:**
- 7 puntos de control arrastrables.
- Cada segmento dibujado en color diferente.
- Polígono de control en gris.

**Ejercicio para el estudiante — Práctica 2.1:**
> Implementa la opción de **agregar/eliminar puntos de control dinámicamente** 
> (clic derecho = agregar punto en posición del cursor, doble clic sobre punto = eliminar).
> Experimenta con al menos 10 puntos de control para formar la letra inicial de tu nombre 
> y guarda una captura.

---

## 2.4 Fractales — `TabFractales.cs`

**Concepto:**  
Un fractal es una figura con **auto-similaridad**: cada parte es una versión escalada 
del todo. Se genera con **recursión** hasta una profundidad dada.

**Árbol binario fractal:**  
En cada llamada se dibuja un segmento, luego se hacen dos llamadas recursivas 
rotadas `±θ`, con longitud reducida (×0.7).

**Copo de nieve de Koch:**  
Se divide cada segmento en 3 partes; la parte central se reemplaza por dos lados 
de un triángulo equilátero. Después de `n` iteraciones, la longitud del perímetro 
tiende a infinito pero el área es finita.

**Dimensión fractal de Koch:**
$$D = \frac{\log 4}{\log 3} \approx 1.26$$

**Lo que muestra el ejemplo:**
- Selección entre árbol fractal y copo de Koch.
- Slider de profundidad (1 a 12 niveles).
- Slider de ángulo para el árbol.

**Ejercicio para el estudiante — Práctica 2.1:**
> Implementa un **tercer fractal: el triángulo de Sierpiński**.
> Usa el método de división de triángulo (IFS: Iterated Function System):
> divide el triángulo en 4, elimina el central, repite recursivamente.
> Agrega un slider de profundidad y experimenta hasta qué nivel la 
> computadora responde en tiempo razonable (≤ 2 segundos).

---

## 2.5 Uso y Creación de Fuentes de Texto — `TabFuentes.cs`

**Concepto:**  
En GDI+ (.NET) el manejo de texto usa las siguientes clases:

| Clase | Propósito |
|---|---|
| `Font` | Define tipografía, tamaño y estilo (Bold, Italic, Underline) |
| `FontFamily` | Familia de fuentes instaladas en el sistema |
| `StringFormat` | Alineación, dirección y recorte del texto |
| `GraphicsPath.AddString` | Convierte texto en vectores (trazos geométricos) |
| `InstalledFontCollection` | Lista todas las fuentes del SO |

Cuando el texto se convierte a `GraphicsPath`, se puede:
- Rellenar con gradientes, patrones o imágenes.
- Aplicar cualquier transformación (rotar, escalar, sesgar).
- Trazar solo el contorno (efecto "outline").

**Lo que muestra el ejemplo:**
1. Tabla de estilos (Regular, Bold, Italic, Bold+Italic).
2. Texto principal con familia, tamaño y estilos seleccionables.
3. Modo **Contorno (Path)**: texto convertido a `GraphicsPath` con relleno degradado.
4. Modo **Texto en curva**: cada carácter se posiciona y rota sobre un arco.

**Ejercicio para el estudiante — Práctica 2.1:**
> Crea una **tarjeta de presentación digital** en un nuevo `Panel` de 600×300 px que use:
> - Al menos 3 familias de fuentes diferentes.
> - Tu nombre como `GraphicsPath` con relleno degradado de dos colores.
> - El nombre de tu carrera como texto siguiendo una curva Bézier (no un arco circular).
> - Un borde decorativo dibujado con `DrawRectangle` o `DrawPath`.
> - Guarda la imagen resultante en PNG usando `panel.DrawToBitmap(...)`.

---

## Cómo ejecutar

```powershell
cd U2Graficacion2D
dotnet run
```

## Estructura del proyecto

```
U2Graficacion2D/
├── Form1.cs              ← Formulario principal con TabControl
├── Form1.Designer.cs
├── Program.cs
└── Tabs/
    ├── TabTraslacion.cs   (2.1.1)
    ├── TabEscalamiento.cs (2.1.2)
    ├── TabRotacion.cs     (2.1.3)
    ├── TabSesgado.cs      (2.1.4)
    ├── TabMatrices.cs     (2.2)
    ├── TabBezier.cs       (2.3.1)
    ├── TabBSpline.cs      (2.3.2)
    ├── TabFractales.cs    (2.4)
    └── TabFuentes.cs      (2.5)
```
