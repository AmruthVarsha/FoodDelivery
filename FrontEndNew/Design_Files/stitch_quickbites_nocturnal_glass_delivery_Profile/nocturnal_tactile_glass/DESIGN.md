---
name: Nocturnal Tactile Glass
colors:
  surface: '#121414'
  surface-dim: '#121414'
  surface-bright: '#37393a'
  surface-container-lowest: '#0c0f0f'
  surface-container-low: '#1a1c1c'
  surface-container: '#1e2020'
  surface-container-high: '#282a2b'
  surface-container-highest: '#333535'
  on-surface: '#e2e2e2'
  on-surface-variant: '#e1bfb5'
  inverse-surface: '#e2e2e2'
  inverse-on-surface: '#2f3131'
  outline: '#a98a80'
  outline-variant: '#594139'
  surface-tint: '#ffb59d'
  primary: '#ffb59d'
  on-primary: '#5d1900'
  primary-container: '#ff6b35'
  on-primary-container: '#5f1900'
  inverse-primary: '#ab3500'
  secondary: '#40e0cb'
  on-secondary: '#003731'
  secondary-container: '#00c3af'
  on-secondary-container: '#004a42'
  tertiary: '#bfc6e0'
  on-tertiary: '#283044'
  tertiary-container: '#9299b2'
  on-tertiary-container: '#2a3146'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#ffdbd0'
  primary-fixed-dim: '#ffb59d'
  on-primary-fixed: '#390c00'
  on-primary-fixed-variant: '#832600'
  secondary-fixed: '#62fae4'
  secondary-fixed-dim: '#3cddc8'
  on-secondary-fixed: '#00201c'
  on-secondary-fixed-variant: '#005047'
  tertiary-fixed: '#dbe2fd'
  tertiary-fixed-dim: '#bfc6e0'
  on-tertiary-fixed: '#131b2e'
  on-tertiary-fixed-variant: '#3f465c'
  background: '#121414'
  on-background: '#e2e2e2'
  surface-variant: '#333535'
typography:
  display:
    fontFamily: Plus Jakarta Sans
    fontSize: 48px
    fontWeight: '800'
    lineHeight: '1.1'
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Plus Jakarta Sans
    fontSize: 32px
    fontWeight: '700'
    lineHeight: '1.2'
  headline-md:
    fontFamily: Plus Jakarta Sans
    fontSize: 24px
    fontWeight: '700'
    lineHeight: '1.3'
  body-lg:
    fontFamily: Plus Jakarta Sans
    fontSize: 18px
    fontWeight: '400'
    lineHeight: '1.5'
  body-md:
    fontFamily: Plus Jakarta Sans
    fontSize: 16px
    fontWeight: '400'
    lineHeight: '1.5'
  body-sm:
    fontFamily: Plus Jakarta Sans
    fontSize: 14px
    fontWeight: '500'
    lineHeight: '1.4'
  label-caps:
    fontFamily: Plus Jakarta Sans
    fontSize: 12px
    fontWeight: '600'
    lineHeight: '1'
    letterSpacing: 0.05em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 8px
  xs: 4px
  sm: 12px
  md: 24px
  lg: 40px
  xl: 64px
  container-padding: 24px
  gutter: 16px
---

## Brand & Style
This design system centers on a "Nocturnal Tactile Glass" aesthetic, engineered to evoke the high-end energy of a modern metropolis at night. It targets urban, tech-savvy consumers who value speed and culinary exploration. 

The style is a sophisticated hybrid: it utilizes **Neomorphism** to provide a sense of physical depth and "pressable" tactile feedback, while **Glassmorphism** creates a sense of light, airy transparency that prevents the dark interface from feeling heavy. The emotional response is one of premium reliability, futuristic efficiency, and sensory indulgence. Large, vibrant background blobs provide a soft organic contrast to the structured, glass-like UI components.

## Colors
The palette is built on a foundation of "Deep Midnight" (#0b1326), providing a high-contrast stage for the brand's vibrant accents. 

- **Primary (Electric Orange):** Used for critical actions, appetite stimulation, and primary branding.
- **Secondary (Neon Cyan):** Used for rewards, tracking indicators, and secondary highlights.
- **Surface:** The glass panels use a highly transparent white tint, relying on backdrop blurs rather than solid fills.
- **Background Blobs:** Implement large, blurred radial gradients using the primary and secondary colors at 10-15% opacity to create a sense of environmental light.

## Typography
Plus Jakarta Sans is the exclusive typeface for this design system, chosen for its modern, geometric clarity and friendly curves. 

- **Weight Strategy:** Use Bold (700) and ExtraBold (800) for titles to establish a strong hierarchy against the complex background. Light (300) and Regular (400) weights should be reserved for secondary text to maintain legibility within glass containers.
- **Contrast:** Primary text is pure white, while secondary text uses a 70% opacity white to maintain hierarchy without introducing new colors.

## Layout & Spacing
The layout follows a strict 8px rhythm. For mobile interfaces, a 24px side margin is mandatory to accommodate the large 24px corner radii of the components, ensuring visual alignment between the edge of the screen and the curve of the cards.

- **Grid:** Use a 12-column grid for desktop and a 4-column grid for mobile.
- **Padding:** Internal card padding should default to 24px (md) to maintain a spacious, premium feel.

## Elevation & Depth
Depth in this design system is achieved through the interplay of two distinct techniques:

1.  **Glass Containers:** These are the primary structural units. They must feature a `backdrop-blur-3xl` and a 1px solid border at `rgba(255, 255, 255, 0.05)`. This border acts as a "specular highlight" on the edge of the glass.
2.  **Tactile Neomorphism:** 
    *   **Outset (Raised):** Use two shadows—a dark shadow (black at 40% opacity) on the bottom-right and a light highlight (white at 5% opacity) on the top-left.
    *   **Inset (Sunken):** Used for inputs and depressed states. Apply internal shadows to give the appearance of the element being carved into the surface.

## Shapes
A consistent 24px (rounded-3xl) corner radius is applied to all primary containers, buttons, and image wrappers. This extreme roundness softens the dark aesthetic and reinforces the "tactile" nature of the design.

Smaller elements like chips or utility buttons may scale down to 12px, but the 24px radius remains the signature structural element of this design system.

## Components
Consistent component styling is vital for the tactile experience:

- **Buttons:** 
    *   **Primary:** Solid #ff6b35 fill with a drop shadow of the same color (spread 10px, opacity 30%) to create a "glow" effect. Text is bold and white.
    *   **Secondary:** Glass-morphic base with a cyan (#44e2cd) border.
- **Input Fields:** These use the neomorphic **inset** shadow effect. The background is slightly darker than the main surface to create a "hollowed-out" appearance.
- **Cards:** Always glass-morphic. For featured food items, the image should slightly "pop" out of the glass container using a subtle outset shadow.
- **Chips:** Highly rounded (pill-shaped) with a `rgba(255, 255, 255, 0.06)` fill and no blur, used for category filtering.
- **Icons:** Material Symbols Outlined, using a 2px stroke weight to match the crispness of the glass borders.
- **Progress Indicators:** Use the secondary cyan (#44e2cd) with a subtle outer glow to represent the delivery path.