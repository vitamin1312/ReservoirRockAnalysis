import { useRef, useState, useEffect } from 'react';
import { Stage, Layer, Line, Image as KonvaImage } from 'react-konva';
import { KonvaEventObject } from 'konva/lib/Node';
import { UpdateMask } from '../../RestAPI/RestAPI';
import { useNavigate } from 'react-router-dom';

type Tool = 'pen' | 'eraser';


interface DrawingCanvasProps {
  objectId: number;
  backgroundImageSrc: string;
  editableImageSrc: string;
}

export default function DrawingCanvas({ objectId, backgroundImageSrc, editableImageSrc }: DrawingCanvasProps) {
  const stageRef = useRef<any>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const [containerSize, setContainerSize] = useState({ width: 1000, height: 700 });

  const [lines, setLines] = useState<{ points: number[]; color: string; tool: Tool; width: number }[]>([]);
  const [tool, setTool] = useState<Tool>('pen');
  const [color, setColor] = useState('#000000');
  const [penWidth, setPenWidth] = useState(2);
  const [eraserWidth, setEraserWidth] = useState(10);
  const isDrawing = useRef(false);

  const [backgroundImage, setBackgroundImage] = useState<HTMLImageElement | null>(null);
  const [editableImage, setEditableImage] = useState<HTMLImageElement | null>(null);

  const [stageScale, setStageScale] = useState(1);
  const [stagePosition, setStagePosition] = useState({ x: 0, y: 0 });

  const navigate = useNavigate();


  const isPanning = useRef(false);
  const lastPointerPosition = useRef<{ x: number; y: number } | null>(null);

  function isInsideImage(x: number, y: number): boolean {
    if (!backgroundImage) return false;
    const scale = getImageScale();
    return (
      x >= 0 &&
      y >= 0 &&
      x <= backgroundImage.width * scale &&
      y <= backgroundImage.height * scale
    );
  }


  // Загрузка изображений
  useEffect(() => {
    const bgImg = new window.Image();
    bgImg.src = backgroundImageSrc;
    bgImg.onload = () => setBackgroundImage(bgImg);

    const editImg = new window.Image();
    editImg.src = editableImageSrc;
    editImg.onload = () => setEditableImage(editImg);
  }, [backgroundImageSrc, editableImageSrc]);

  // Отслеживаем размер контейнера для подгонки stage
  useEffect(() => {
    function updateSize() {
      if (containerRef.current) {
        setContainerSize({
          width: containerRef.current.clientWidth,
          height: containerRef.current.clientHeight,
        });
      }
    }
    updateSize();

    window.addEventListener('resize', updateSize);
    return () => window.removeEventListener('resize', updateSize);
  }, []);

  const handleWheel = (e: KonvaEventObject<WheelEvent>) => {
    e.evt.preventDefault();

    const scaleBy = 1.05;
    const stage = e.target.getStage();
    if (!stage) return;

    const oldScale = stageScale;
    const pointer = stage.getPointerPosition();
    if (!pointer) return;

    const mousePointTo = {
      x: (pointer.x - stagePosition.x) / oldScale,
      y: (pointer.y - stagePosition.y) / oldScale,
    };

    const direction = e.evt.deltaY > 0 ? -1 : 1;
    const newScale = direction > 0 ? oldScale * scaleBy : oldScale / scaleBy;
    setStageScale(newScale);

    const newPos = {
      x: pointer.x - mousePointTo.x * newScale,
      y: pointer.y - mousePointTo.y * newScale,
    };
    setStagePosition(newPos);
  };

  // Подгонка размера изображения и масштабирование чтобы поместилось
  const getImageScale = () => {
    if (!backgroundImage) return 1;
    const scaleX = containerSize.width / backgroundImage.width;
    const scaleY = containerSize.height / backgroundImage.height;
    return Math.min(scaleX, scaleY);
  };

    // Отключаем контекстное меню ПКМ
  const handleContextMenu = (e: KonvaEventObject<MouseEvent>) => {
    e.evt.preventDefault();
  };

  // Нажали мышь
  const handleMyMouseDown = (e: KonvaEventObject<MouseEvent>) => {
    const stage = e.target.getStage();
    if (!stage) return;

    const pos = stage.getPointerPosition();
    if (!pos) return;

    if (e.evt.button === 2) {
      // ПКМ — активируем панорамирование
      isPanning.current = true;
      lastPointerPosition.current = pos;
      return;
    }

    if (e.evt.button !== 0) return; // Игнорируем все кроме ЛКМ и ПКМ

    isDrawing.current = true;

    // Координаты с учетом масштаба и позиции
    const realX = (pos.x - stagePosition.x) / stageScale;
    const realY = (pos.y - stagePosition.y) / stageScale;
    if (!isInsideImage(realX, realY)) return;

    setLines([
      ...lines,
      {
        points: [realX, realY],
        color,
        tool,
        width: tool === 'pen' ? penWidth : eraserWidth,
      },
    ]);
  };


  // Движение мыши
  const handleMouseMyMove = (e: KonvaEventObject<MouseEvent>) => {
    if (!isPanning.current) return;
    const stage = e.target.getStage();
    if (!stage) return;

    const pointer = stage.getPointerPosition();
    if (!pointer || !lastPointerPosition.current) return;

    // Смещение мыши
    const dx = pointer.x - lastPointerPosition.current.x;
    const dy = pointer.y - lastPointerPosition.current.y;

    setStagePosition((pos) => ({
      x: pos.x + dx,
      y: pos.y + dy,
    }));

    lastPointerPosition.current = pointer;
  };

  // Отпустили мышь
  const handleMyMouseUp = (e: KonvaEventObject<MouseEvent>) => {
    if (e.evt.button === 2) {
      isPanning.current = false;
      lastPointerPosition.current = null;
    }
  };

  // Рисование
  const handleMouseDown = (e: any) => {
    if (e.evt.button !== 0) return; // Только ЛКМ

    isDrawing.current = true;
    const pos = e.target.getStage().getPointerPosition();
    if (!pos) return;

    const realX = (pos.x - stagePosition.x) / stageScale;
    const realY = (pos.y - stagePosition.y) / stageScale;

    if (!isInsideImage(realX, realY)) return;

    setLines([...lines, {
      points: [realX, realY],
      color,
      tool,
      width: tool === 'pen' ? penWidth : eraserWidth
    }]);
  };

  const handleMouseMove = (e: any) => {
      if (!isDrawing.current || e.evt.buttons !== 1) return;

      const pos = e.target.getStage().getPointerPosition();
      if (!pos) return;

      const realX = (pos.x - stagePosition.x) / stageScale;
      const realY = (pos.y - stagePosition.y) / stageScale;

      // 🔧 Добавляем проверку здесь
      if (!isInsideImage(realX, realY)) return;

      const lastLine = lines[lines.length - 1];
      if (!lastLine) return;

      const newLines = lines.slice(0, -1);
      newLines.push({
        ...lastLine,
        points: [...lastLine.points, realX, realY],
      });
      setLines(newLines);
    };

    const handleMouseUp = (e: any) => {
      if (e.evt.button === 0) {
        isDrawing.current = false;
      }
    };

const handleSave = (id: number) => {
  if (!editableImage) return;

  const realWidth = editableImage.width;
  const realHeight = editableImage.height;

  const stageNode = stageRef.current?.getStage();
  const container = stageNode?.container();
  const displayedCanvasWidth = container?.clientWidth || realWidth;
  const displayedCanvasHeight = container?.clientHeight || realHeight;

  const scaleX = realWidth / displayedCanvasWidth;
  const scaleY = realHeight / displayedCanvasHeight;

  const canvas = document.createElement('canvas');
  canvas.width = realWidth;
  canvas.height = realHeight;
  const ctx = canvas.getContext('2d');
  if (!ctx) return;

    ctx.imageSmoothingEnabled = true;
    ctx.imageSmoothingQuality = 'high';

    ctx.drawImage(editableImage, 0, 0, realWidth, realHeight);

    lines.forEach(line => {
      ctx.globalCompositeOperation = line.tool === 'eraser' ? 'destination-out' : 'source-over';
      ctx.strokeStyle = line.color;
      ctx.lineWidth = line.width * scaleX;

      ctx.lineJoin = 'round';
      ctx.lineCap = 'round';

      ctx.beginPath();
      const pts = line.points;
      if (pts.length >= 2) {
        ctx.moveTo(pts[0] * scaleX, pts[1] * scaleY);
        for (let i = 2; i < pts.length; i += 2) {
          ctx.lineTo(pts[i] * scaleX, pts[i + 1] * scaleY);
        }
      }
      ctx.stroke();
    });

    const dataURL = canvas.toDataURL('image/png');
    UpdateMask(dataURL, id);
  };



  return (
    <div className="flex flex-col h-full" ref={containerRef} style={{ position: 'relative' }}>
      {/* Панель инструментов */}
      <div className="flex items-center h-16 px-8 border-b border-gray-300 bg-gray-50 justify-between">
      {/* Левая часть с кнопками и настройками */}
      <div className="flex items-center gap-4">
        <button
          onClick={() => setTool('pen')}
          className={`px-3 py-1 rounded ${
            tool === 'pen' ? 'font-bold bg-blue-200 text-blue-900' : 'font-normal text-gray-700 hover:bg-gray-200'
          }`}
        >
          Карандаш
        </button>
        <button
          onClick={() => setTool('eraser')}
          className={`px-3 py-1 rounded ${
            tool === 'eraser' ? 'font-bold bg-blue-200 text-blue-900' : 'font-normal text-gray-700 hover:bg-gray-200'
          }`}
        >
          Ластик
        </button>

        {tool === 'pen' && (
          <>
            <input
              type="color"
              value={color}
              onChange={(e) => setColor(e.target.value)}
              className="w-10 h-8 p-0 border-none cursor-pointer"
              title="Выбор цвета"
            />
            <label className="flex items-center gap-2">
              <span className="text-sm text-gray-700">Толщина:</span>
              <input
                type="range"
                min={1}
                max={20}
                value={penWidth}
                onChange={(e) => setPenWidth(parseInt(e.target.value))}
                className="w-24 cursor-pointer"
              />
              <span className="w-6 text-right text-sm">{penWidth}</span>
            </label>
          </>
        )}

        {tool === 'eraser' && (
          <label className="flex items-center gap-2">
            <span className="text-sm text-gray-700">Толщина:</span>
            <input
              type="range"
              min={5}
              max={50}
              value={eraserWidth}
              onChange={(e) => setEraserWidth(parseInt(e.target.value))}
              className="w-24 cursor-pointer"
            />
            <span className="w-6 text-right text-sm">{eraserWidth}</span>
          </label>
        )}
      </div>

      {/* Правая часть: кнопка сохранить */}
    <button
        onClick={() => {
          handleSave(objectId);
          navigate("/");
        }}
        className="px-3 py-2 rounded bg-green-500 text-white hover:bg-green-600 mr-4 my-2 self-center"
      >
        Сохранить
    </button>
    </div>

      {/* Холст */}
      <div className="flex-grow border border-gray-300">
        <Stage
          ref={stageRef}
          width={containerSize.width}
          height={containerSize.height}
          scaleX={stageScale}
          scaleY={stageScale}
          x={stagePosition.x}
          y={stagePosition.y}
          onWheel={handleWheel}
          onContextMenu={handleContextMenu}
          onMouseDown={(e) => {
            handleMouseDown(e);
            handleMyMouseDown(e);
          }}
          onMouseMove={(e) => {
            handleMouseMove(e);
            handleMouseMyMove(e);
          }}
          onMouseUp={(e) => {
            handleMouseUp(e);
            handleMyMouseUp(e);
          }}
          style={{ background: '#fff', touchAction: 'none' }}
        >
          <Layer>
            {backgroundImage && (
              <KonvaImage
                image={backgroundImage}
                width={backgroundImage.width}
                height={backgroundImage.height}
                scaleX={getImageScale()}
                scaleY={getImageScale()}
              />
            )}
          </Layer>

          <Layer>
            {editableImage && (
              <KonvaImage
                image={editableImage}
                width={editableImage.width}
                height={editableImage.height}
                scaleX={getImageScale()}
                scaleY={getImageScale()}
              />
            )}

            {lines.map((line, i) => (
              <Line
                key={i}
                points={line.points}
                stroke={line.color}
                strokeWidth={line.width}
                tension={0.5}
                lineCap="round"
                lineJoin="round"
                globalCompositeOperation={
                  line.tool === 'eraser' ? 'destination-out' : 'source-over'
                }
              />
            ))}
          </Layer>
        </Stage>
      </div>
    </div>
  );
}
