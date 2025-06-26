/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./AppWeb.Client/**/*.{razor,cshtml,html}",
    "./AppWeb.Server/Components/**/*.{razor,cshtml,html}",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
};