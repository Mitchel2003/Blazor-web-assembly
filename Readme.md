## CSR (Client-Side Rendering):
 `En CSR, el navegador descarga una página HTML vacía y luego ejecuta JavaScript para renderizar el contenido. Esto puede resultar en una experiencia más interactiva y rápida una vez que la página está cargada, pero puede tener un tiempo de carga inicial más lento y afectar negativamente al SEO.`

 `Es adecuado para aplicaciones web altamente interactivas donde la velocidad de carga inicial no es tan crítica.`

## SSR (Server-Side Rendering):
 `En SSR, el servidor genera el HTML completo de la página antes de enviarlo al navegador. Esto resulta en un tiempo de carga inicial más rápido y una mejor indexación por los motores de búsqueda (SEO), pero puede requerir más recursos del servidor y puede ser más complejo de implementar.`

 `Es beneficioso para sitios web donde el SEO y la velocidad de carga inicial son importantes.`

## SSG (Static Site Generation): 
 `En SSG, las páginas HTML se generan durante la compilación y se sirven como archivos estáticos. Esto resulta en tiempos de carga extremadamente rápidos y una excelente experiencia de usuario, pero no es adecuado para sitios web con contenido altamente dinámico o que cambia con frecuencia.`

 `Ideal para sitios web con contenido estático, como blogs, documentación, etc.`

## En resumen: 
 `CSR: Rápido después de la carga inicial, ideal para aplicaciones interactivas.`
 `SSR: Carga rápida inicial, buen SEO, pero puede ser más complejo.`
 `SSG: Carga muy rápida, ideal para contenido estático, pero no para contenido dinámico.`

### ---------------------------------------------------------------------------------------------------- ###

### -----------------------------------------------Readme----------------------------------------------- ###
## Prompt:
`Mira primero que todo quiero felicitarte y darte el mérito porque realmente has estado estupenda déjame decirte que tienes el nivel de maestría que denota más de 10 años de experiencia en el sector del desarrollo y la construcción de sistemas robustos capaces de responder a problemas reales de manera ingeniosa eficiente y escalable y ha sido increíble todo lo que has hecho o sea se nota que has trabajado en empresas muy grandes y reconocidas`

## Compile tailwindcss
`npx tailwindcss -i ./tailwind.input.css -o ./AppWeb.Client/wwwroot/css/app.css --minify`